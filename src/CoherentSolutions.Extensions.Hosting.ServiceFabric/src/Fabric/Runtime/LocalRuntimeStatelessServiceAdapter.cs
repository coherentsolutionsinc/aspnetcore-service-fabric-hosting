using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories;
using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class LocalRuntimeStatelessServiceAdapter
    {
        private readonly StatelessServiceAccessor service;

        private readonly IStatelessServicePartition partition;

        private readonly ILogger logger;

        public LocalRuntimeStatelessServiceAdapter(
            StatelessService service,
            IStatelessServicePartition partition,
            ILogger logger)
        {
            this.service = new StatelessServiceAccessor(service ?? throw new ArgumentNullException(nameof(service)));
            this.partition = partition ?? throw new ArgumentNullException(nameof(partition));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OpenAsync()
        {
            this.SetPartition();

            await this.ExecutingOpenCommunicationListenersAsync();

            var runAsyncTask = Task.Run(this.ExecuteRunAsync);
            var openAsyncTask = Task.Run(this.ExecuteOpenAsync);

            await Task.WhenAll(openAsyncTask, runAsyncTask);
        }

        private void SetPartition()
        {
            this.logger.LogInformation("Setting Partition...");

            try
            {
                this.service.Partition = this.partition;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "Unable to set Partition because of unexpected exception");

                throw;
            }

            this.logger.LogInformation("Done setting Partition");
        }

        private async Task ExecutingOpenCommunicationListenersAsync()
        {
            this.logger.LogInformation("Openning Communication Listeners...");

            try
            {
                var instanceListeners = this.service.CreateServiceInstanceListeners();
                if (instanceListeners is null)
                {
                    this.logger.LogInformation("No Communication Listeners found");
                    return;
                }

                var listeners = new HashSet<string>();

                foreach (var instanceListener in instanceListeners)
                {
                    if (instanceListener is null)
                    {
                        this.logger.LogInformation("Skipping null Instance Listener");
                        continue;
                    }

                    var name = instanceListener.Name;
                    if (!listeners.Add(name))
                    {
                        throw new InvalidOperationException($"The communication listener with '{name}' has been already registered.");
                    }

                    var communicationListener = instanceListener.CreateCommunicationListener(this.service.Instance.Context);
                    if (communicationListener is null)
                    {
                        this.logger.LogInformation("Skipping null Communication Listener");
                        continue;
                    }

                    this.logger.LogInformation($"Opening '{name}' Communication Listener...");

                    var endpoint = await communicationListener.OpenAsync(default);

                    this.logger.LogInformation($"Done opening '{name}' Communication Listener");

                    this.logger.LogInformation($"The host is listening on: {endpoint}");
                }
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "Unable to open Communication Listeners because of unexpected exception.");

                throw;
            }

            this.logger.LogInformation("Done opening Communication Listeners.");
        }

        private async Task ExecuteOpenAsync()
        {
            this.logger.LogInformation("Executing OpenAsync...");

            try
            {
                await this.service.OpenAsync(default);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "Unable to execute OpenAsync because of unexpected exception");

                throw;
            }

            this.logger.LogInformation("Done executing OpenAsync");
        }

        private async Task ExecuteRunAsync()
        {
            this.logger.LogInformation("Executing RunAsync...");

            try
            {
                await this.service.RunAsync(default);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e, "Unable to execute RunAsync because of unexpected exception");

                throw;
            }

            this.logger.LogInformation("Done executing RunAsync");
        }
    }
}