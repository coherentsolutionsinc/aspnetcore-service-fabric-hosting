using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostGenericListenerReplicaTemplate
        : ServiceHostGenericListenerReplicaTemplate<
              IStatelessService,
              IStatelessServiceHostGenericListenerReplicaTemplateParameters,
              IStatelessServiceHostGenericListenerReplicaTemplateConfigurator,
              ServiceInstanceListener>,
          IStatelessServiceHostGenericListenerReplicaTemplate
    {
        private class StatelessListenerParameters
            : GenericListenerParameters,
              IStatelessServiceHostGenericListenerReplicaTemplateParameters,
              IStatelessServiceHostGenericListenerReplicaTemplateConfigurator
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