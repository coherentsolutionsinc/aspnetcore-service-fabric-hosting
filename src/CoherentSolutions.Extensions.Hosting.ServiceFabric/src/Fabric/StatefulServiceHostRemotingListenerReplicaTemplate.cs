using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
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

            parameters.ConfigureDependencies(
                services =>
                {
                    services.Add(new ServiceDescriptor(typeof(IReliableStateManager), service.GetReliableStateManager()));
                });

            this.UpstreamConfiguration(parameters);

            var factoryFunc = this.CreateFactory(parameters);

            return new ServiceReplicaListener(context => factoryFunc(service), parameters.EndpointName, parameters.ListenerOnSecondary);
        }
    }
}