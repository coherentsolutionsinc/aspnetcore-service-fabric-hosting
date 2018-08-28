using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class MockStatefulServiceReplica
    {
        private readonly Func<StatefulServiceContext, StatefulServiceBase> serviceFactory;

        private readonly StatefulServiceContext serviceContext;

        private StatefulServiceBase serviceReplica;

        private ICommunicationListener[] communicationListeners;

        private Task runAsyncTask;

        private CancellationTokenSource runAsyncCancellationTokenSource;

        private bool running;

        private TaskCompletionSource<bool> startTaskSource;

        private TaskCompletionSource<bool> stopTaskSource;

        public MockStatefulServiceReplica(
            Func<StatefulServiceContext, StatefulServiceBase> serviceFactory,
            StatefulServiceContext serviceContext)
        {
            this.serviceFactory = serviceFactory
             ?? throw new ArgumentNullException(nameof(serviceFactory));

            this.serviceContext = serviceContext
             ?? throw new ArgumentNullException(nameof(serviceContext));
        }

        public Task StartAsync()
        {
            return this.InitiateStartupSequence();
        }

        public Task StopAsync()
        {
            return this.InitiateShutdownSequence();
        }

        private async Task InitiateStartupSequence()
        {
            var skip = true;
            lock (this.serviceFactory)
            {
                if (!this.running)
                {
                    this.serviceReplica = this.serviceFactory(this.serviceContext);

                    this.running = true;
                    this.startTaskSource = new TaskCompletionSource<bool>();
                    this.stopTaskSource = new TaskCompletionSource<bool>();

                    skip = false;
                }
            }

            if (skip)
            {
                await this.startTaskSource.Task;
                return;
            }

            Injector.InjectProperty(this.serviceReplica, "Partition", new MockStatefulServicePartition(), true);

            var cancellationTokenSource = new CancellationTokenSource();

            await this.serviceReplica.InvokeOnOpenAsync(ReplicaOpenMode.New, cancellationTokenSource.Token);

            // ReSharper disable once MethodSupportsCancellation
            var openListenersTask = Task.Run(
                async () =>
                {
                    this.communicationListeners = this.serviceReplica
                       .InvokeCreateServiceReplicaListeners()
                       .Select(l => l.CreateCommunicationListener(this.serviceContext))
                       .ToArray();

                    var communicationListenersOpenTasks = new Task[this.communicationListeners.Length];
                    for (var i = 0; i < this.communicationListeners.Length; ++i)
                    {
                        communicationListenersOpenTasks[i] = this.communicationListeners[i].OpenAsync(cancellationTokenSource.Token);
                    }

                    await Task.WhenAll(communicationListenersOpenTasks);
                });

            this.runAsyncCancellationTokenSource = new CancellationTokenSource();
            this.runAsyncTask = this.serviceReplica.InvokeRunAsync(this.runAsyncCancellationTokenSource.Token);

            #pragma warning disable 4014
            this.runAsyncTask
                // ReSharper disable once MethodSupportsCancellation
               .ContinueWith(
                    #pragma warning restore 4014
                    async t =>
                    {
                        await this.InitiateShutdownSequence();
                    },
                    TaskContinuationOptions.OnlyOnFaulted);

            await openListenersTask;

            await this.serviceReplica.InvokeOnChangeRoleAsync(ReplicaRole.Primary, cancellationTokenSource.Token);

            this.startTaskSource.SetResult(true);
        }

        private async Task InitiateShutdownSequence()
        {
            var skip = true;
            lock (this.serviceFactory)
            {
                if (this.running)
                {
                    this.running = false;

                    skip = false;
                }
            }

            if (skip)
            {
                await this.stopTaskSource.Task;
                return;
            }

            var cancellationTokenSource = new CancellationTokenSource();

            var closeListenersTask = Task.Run(
                async () =>
                {
                    var communicationListenersCloseTasks = new Task[this.communicationListeners.Length];
                    for (var i = 0; i < this.communicationListeners.Length; ++i)
                    {
                        communicationListenersCloseTasks[i] = this.communicationListeners[i].CloseAsync(cancellationTokenSource.Token);
                    }

                    await Task.WhenAll(communicationListenersCloseTasks);
                });

            this.runAsyncCancellationTokenSource.CancelAfter(100);

            Exception exception = null;
            try
            {
                await this.runAsyncTask;
            }
            catch (OperationCanceledException)
            {
                // skip the cancellation exception here.
            }
            catch (Exception e)
            {
                exception = e;
            }

            await closeListenersTask;

            await this.serviceReplica.InvokeOnChangeRoleAsync(ReplicaRole.None, cancellationTokenSource.Token);

            await this.serviceReplica.InvokeOnCloseAsync(cancellationTokenSource.Token);

            if (exception == null)
            {
                this.stopTaskSource.SetResult(true);
            }
            else
            {
                this.stopTaskSource.SetException(exception);
            }
        }
    }
}