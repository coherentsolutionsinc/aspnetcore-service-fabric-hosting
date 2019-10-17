using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostAspNetCoreListenerReplicaTemplate
        : ServiceHostAspNetCoreListenerReplicaTemplate<
              IStatelessService,
              IStatelessServiceHostAspNetCoreListenerReplicaTemplateParameters,
              IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
              ServiceInstanceListener>,
          IStatelessServiceHostAspNetCoreListenerReplicaTemplate
    {
        private class StatelessListenerParameters
            : AspNetCoreListenerParameters,
              IStatelessServiceHostAspNetCoreListenerReplicaTemplateParameters,
              IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        {
        }

        public override ServiceInstanceListener Activate(
            IStatelessService service)
        {
            var parameters = new StatelessListenerParameters();

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateFactory(parameters);

            return new ServiceInstanceListener(context => factory(service), parameters.EndpointName);
        }
    }
}