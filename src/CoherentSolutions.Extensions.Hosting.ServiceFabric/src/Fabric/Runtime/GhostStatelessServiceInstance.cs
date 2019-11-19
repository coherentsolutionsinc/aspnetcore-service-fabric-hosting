using System;
using System.Collections.Generic;
using System.Fabric;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class GhostStatelessServiceInstance
    {
        private static readonly Lazy<Func<StatelessService, IEnumerable<ServiceInstanceListener>>> createInstanceListeners;

        private static readonly Lazy<Func<StatelessService, CancellationToken, Task>> runAsync;

        private static readonly Lazy<Func<StatelessService, CancellationToken, Task>> onOpenAsync;

        private readonly StatelessService service;

        private readonly IStatelessServicePartition partition;

        private readonly ILogger logger;

        static GhostStatelessServiceInstance()
        {
            createInstanceListeners = new Lazy<Func<StatelessService, IEnumerable<ServiceInstanceListener>>>(
                () =>
                {
                    var m = typeof(StatelessService).Query().Method("CreateServiceInstanceListeners").Instance().NonPublic().Get();
                    return service => (IEnumerable<ServiceInstanceListener>)m.Invoke(service, null);
                },
                true);
            onOpenAsync = new Lazy<Func<StatelessService, CancellationToken, Task>>(
                () =>
                {
                    var m = typeof(StatelessService).Query().Method("OnOpenAsync").Params(typeof(CancellationToken)).Instance().NonPublic().Get();
                    return (
                        service,
                        cancellationToken) => (Task)m.Invoke(service, new object[] { cancellationToken });
                });
            runAsync = new Lazy<Func<StatelessService, CancellationToken, Task>>(
                () =>
                {
                    var m = typeof(StatelessService).Query().Method("RunAsync").Params(typeof(CancellationToken)).Instance().NonPublic().Get();
                    return (
                        service,
                        cancellationToken) => (Task)m.Invoke(service, new object[] { cancellationToken });
                });
        }

        public GhostStatelessServiceInstance(
            StatelessService service,
            ILogger logger)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartupAsync()
        {
            await this.OpenCommunicationListeners();

            var runAsyncTask = Task.Run(this.ExecuteRunAsync);
            var openAsyncTask = Task.Run(this.ExecuteOpenAsync);

            await Task.WhenAll(openAsyncTask, runAsyncTask);
        }

        private async Task OpenCommunicationListeners()
        {
            this.logger.LogInformation("Opening communication listeners");

            try
            {
                var instanceListeners = createInstanceListeners.Value(this.service);
                if (instanceListeners is null)
                {
                    this.logger.LogInformation("No communication listeners found.");
                    return;
                }

                var listeners = new HashSet<string>();

                foreach (var instanceListener in instanceListeners)
                {
                    if (instanceListener is null)
                    {
                        this.logger.LogInformation("Skipping null communication listener.");
                        continue;
                    }

                    var name = instanceListener.Name;

                    var communicationListener = instanceListener.CreateCommunicationListener(this.service.Context);
                    if (communicationListener is null)
                    {
                        this.logger.LogInformation("Skipping null communication listener.");
                        continue;
                    }

                    this.logger.LogInformation($"Opening '{name}' communication listener.");

                    var endpoint = await communicationListener.OpenAsync(default);

                    this.logger.LogInformation($"The host is listening on: {endpoint}");
                    this.logger.LogInformation($"Done opening '{name}' communication listener.");

                    if (!listeners.Add(name))
                    {
                        throw new InvalidOperationException($"The communication listener with '{name}' has been already registered.");
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "Unable to open communication listeners because of unexpected exception.");

                throw;
            }

            this.logger.LogInformation("Done opening communication listeners.");
        }

        private async Task ExecuteOpenAsync()
        {
            this.logger.LogInformation("Executing OpenAsync");

            try
            {
                await onOpenAsync.Value(this.service, default);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "Unable to execute RunAsync because of unexpected exception.");

                throw;
            }

            this.logger.LogInformation("Done executing OpenAsync");
        }

        private async Task ExecuteRunAsync()
        {
            this.logger.LogInformation("Executing RunAsync");

            try
            {
                await runAsync.Value(this.service, default);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "Unable to execute RunAsync because of unexpected exception.");

                throw;
            }

            this.logger.LogInformation("Done executing RunAsync");
        }
    }
}