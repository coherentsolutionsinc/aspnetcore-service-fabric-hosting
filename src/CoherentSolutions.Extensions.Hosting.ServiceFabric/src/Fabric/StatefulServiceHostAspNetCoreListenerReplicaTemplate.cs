using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostAspNetCoreListenerReplicaTemplate
        : ServiceHostAspNetCoreListenerReplicaTemplate<
              IStatefulService,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplateParameters,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
              ServiceReplicaListener>,
          IStatefulServiceHostAspNetCoreListenerReplicaTemplate
    {
        private class StatefulListenerParameters
            : AspNetCoreListenerParameters,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplateParameters,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        {
            public bool ListenerOnSecondary { get; private set; }

            public StatefulListenerParameters()
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
            var parameters = new StatefulListenerParameters();

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

            var factory = this.CreateCommunicationListenerFunc(service, parameters);

            return new ServiceReplicaListener(factory, parameters.EndpointName, parameters.ListenerOnSecondary);
        }
    }
}