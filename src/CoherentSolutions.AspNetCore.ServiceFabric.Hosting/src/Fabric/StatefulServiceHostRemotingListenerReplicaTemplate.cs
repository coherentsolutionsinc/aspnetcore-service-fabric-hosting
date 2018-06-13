using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class StatefulServiceHostRemotingListenerReplicaTemplate
        : ServiceHostRemotingListenerReplicaTemplate<
              IStatefulService,
              IStatefulServiceHostRemotingListenerReplicaTemplateParameters,
              IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator,
              ServiceReplicaListener>,
          IStatefulServiceHostRemotingListenerReplicaTemplate
    {
        private class StatefulListenerParameters
            : RemotingListenerParameters,
              IStatefulServiceHostRemotingListenerReplicaTemplateParameters,
              IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator
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

            this.UpstreamConfiguration(parameters);

            var factoryFunc = this.CreateCommunicationListenerFunc(service, parameters);

            return new ServiceReplicaListener(factoryFunc, parameters.EndpointName, parameters.ListenerOnSecondary);
        }
    }
}