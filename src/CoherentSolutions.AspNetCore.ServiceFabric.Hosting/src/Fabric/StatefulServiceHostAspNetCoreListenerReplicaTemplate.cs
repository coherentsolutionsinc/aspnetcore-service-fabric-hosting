using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class StatefulServiceHostAspNetCoreListenerReplicaTemplate
        : ServiceHostAspNetCoreListenerReplicaTemplate<
              IStatefulService,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplateParameters,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
              ServiceReplicaListener>,
          IStatefulServiceHostAspNetCoreListenerReplicaTemplate
    {
        private class StatefulParameters
            : Parameters,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplateParameters,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        {
            public bool ListenerOnSecondary { get; private set; }

            public StatefulParameters()
            {
                this.ListenerOnSecondary = false;
            }

            public void UseListenerOnSecondary()
            {
                this.ListenerOnSecondary = true;
            }
        }

        public override ServiceReplicaListener Activate(
            IStatefulService service)
        {
            var parameters = new StatefulParameters();

            parameters.ConfigureWebHost(
                builder =>
                {
                    builder.ConfigureServices(
                        services =>
                        {
                            services.Add(new ServiceDescriptor(typeof(IReliableStateManager), service.GetReliableStateManager()));
                        });
                });

            this.UpstreamConfiguration(parameters);

            var factoryFunc = this.CreateAspNetCoreCommunicationListenerFunc(service, parameters);

            return new ServiceReplicaListener(factoryFunc, parameters.EndpointName, parameters.ListenerOnSecondary);
        }
    }
}