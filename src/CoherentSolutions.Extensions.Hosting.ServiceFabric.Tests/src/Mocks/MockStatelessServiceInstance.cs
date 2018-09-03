using System.Linq;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Tools;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockStatelessServiceInstance
    {
        private readonly StatelessService serviceInstance;

        public MockStatelessServiceInstance(
            StatelessService instance)
        {
            this.serviceInstance = instance;

            Injector.InjectProperty(this.serviceInstance, "Partition", new MockStatelessServicePartition(), true);
        }

        public async Task InitiateStartupSequenceAsync()
        {
            var openListenersTask = Task.Run(
                async () =>
                {
                    var communicationListeners = this.serviceInstance
                       .InvokeCreateServiceInstanceListeners()
                       .Select(l => l.CreateCommunicationListener(this.serviceInstance.Context))
                       .ToArray();

                    var communicationListenersOpenTasks = new Task[communicationListeners.Length];
                    for (var i = 0; i < communicationListeners.Length; ++i)
                    {
                        communicationListenersOpenTasks[i] = communicationListeners[i].OpenAsync(default);
                    }

                    await Task.WhenAll(communicationListenersOpenTasks);
                });

            var runAsyncTask = this.serviceInstance.InvokeRunAsync();

            await openListenersTask;

            var openAsyncTask = this.serviceInstance.InvokeOnOpenAsync();

            await Task.WhenAll(openAsyncTask, runAsyncTask);
        }

        public async Task InitiateShutdownSequenceAsync()
        {
            var closeListenersTask = Task.Run(
                async () =>
                {
                    var communicationListeners = this.serviceInstance
                       .InvokeCreateServiceInstanceListeners()
                       .Select(l => l.CreateCommunicationListener(this.serviceInstance.Context))
                       .ToArray();

                    var communicationListenersCloseTasks = new Task[communicationListeners.Length];
                    for (var i = 0; i < communicationListeners.Length; ++i)
                    {
                        communicationListenersCloseTasks[i] = communicationListeners[i].CloseAsync(default);
                    }

                    await Task.WhenAll(communicationListenersCloseTasks);
                });

            await closeListenersTask;

            await this.serviceInstance.InvokeOnCloseAsync();
        }
    }
}