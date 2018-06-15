using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostRemotingListenerReplicaTemplate
        : ServiceHostRemotingListenerReplicaTemplate<
              IStatelessService,
              IStatelessServiceHostRemotingListenerReplicaTemplateParameters,
              IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator,
              ServiceInstanceListener>,
          IStatelessServiceHostRemotingListenerReplicaTemplate
    {
        private class StatelessListenerParameters
            : RemotingListenerParameters,
              IStatelessServiceHostRemotingListenerReplicaTemplateParameters,
              IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator
        {
        }

        public override ServiceInstanceListener Activate(
            IStatelessService service)
        {
            var parameters = new StatelessListenerParameters();

            this.UpstreamConfiguration(parameters);

            var factoryFunc = this.CreateCommunicationListenerFunc(service, parameters);

            return new ServiceInstanceListener(factoryFunc, parameters.EndpointName);
        }
    }
}