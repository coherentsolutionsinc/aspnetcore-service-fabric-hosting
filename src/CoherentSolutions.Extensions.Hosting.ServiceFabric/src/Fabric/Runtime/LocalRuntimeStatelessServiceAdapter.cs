using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Services;

using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class LocalRuntimeStatelessServiceAdapter
    {
        private readonly StatelessServiceAccessor<StatelessService> service;

        private readonly ILogger logger;

        public LocalRuntimeStatelessServiceAdapter(
            StatelessServiceAccessor<StatelessService> service,
            ILogger logger)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OpenAsync()
        {
            await this.ExecutingOpenCommunicationListenersAsync();

            var runAsyncTask = Task.Run(this.ExecuteRunAsync);
            var openAsyncTask = Task.Run(this.ExecuteOpenAsync);

            await Task.WhenAll(openAsyncTask, runAsyncTask);
        }

        private async Task ExecutingOpenCommunicationListenersAsync()
        {
            try
            {
                var instanceListeners = this.service.CreateServiceInstanceListeners();
                if (instanceListeners is null)
                {
                    this.logger.LogWarning("No communication listeners were returned by stateless service");
                    return;
                }

                var listeners = new HashSet<string>();
                foreach (var instanceListener in instanceListeners)
                {
                    if (instanceListener is null)
                    {
                        continue;
                    }

                    var name = instanceListener.Name;
                    if (!listeners.Add(name))
                    {
                        throw new InvalidOperationException($"Communication listener ({name}) has been already registered");
                    }

                    var communicationListener = instanceListener.CreateCommunicationListener(this.service.Instance.Context);
                    if (communicationListener is null)
                    {
                        continue;
                    }

                    var endpoint = await communicationListener.OpenAsync(default);

                    this.logger.LogInformation($"{name}: {endpoint}");
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to open communication listeners because of unexpected exception");

                throw;
            }
        }

        private async Task ExecuteOpenAsync()
        {
            try
            {
                await this.service.OpenAsync(default);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to execute OpenAsync because of unexpected exception");

                throw;
            }
        }

        private async Task ExecuteRunAsync()
        {
            try
            {
                await this.service.RunAsync(default);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to execute RunAsync because of unexpected exception");

                throw;
            }
        }
    }
}