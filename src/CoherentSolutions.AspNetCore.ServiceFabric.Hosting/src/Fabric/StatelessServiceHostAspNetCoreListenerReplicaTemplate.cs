using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class StatelessServiceHostAspNetCoreListenerReplicaTemplate
        : ServiceHostAspNetCoreListenerReplicaTemplate<
              IStatelessService,
              IStatelessServiceHostAspNetCoreListenerReplicaTemplateParameters,
              IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
              ServiceInstanceListener>,
          IStatelessServiceHostAspNetCoreListenerReplicaTemplate
    {
        private class StatelessParameters
            : Parameters,
              IStatelessServiceHostAspNetCoreListenerReplicaTemplateParameters,
              IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        {
        }

        public override ServiceInstanceListener Activate(
            IStatelessService service)
        {
            var parameters = new StatelessParameters();

            this.UpstreamConfiguration(parameters);

            var factoryFunc = this.CreateAspNetCoreCommunicationListenerFunc(service, parameters);

            return new ServiceInstanceListener(factoryFunc, parameters.EndpointName);
        }
    }
}