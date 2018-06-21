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

            var factory = this.CreateCommunicationListenerFunc(service, parameters);

            return new ServiceInstanceListener(factory, parameters.EndpointName);
        }
    }
}