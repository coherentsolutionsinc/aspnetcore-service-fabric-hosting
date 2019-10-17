using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostGenericListenerReplicaTemplate
        : ServiceHostGenericListenerReplicaTemplate<
              IStatefulService,
              IStatefulServiceHostGenericListenerReplicaTemplateParameters,
              IStatefulServiceHostGenericListenerReplicaTemplateConfigurator,
              ServiceReplicaListener>,
          IStatefulServiceHostGenericListenerReplicaTemplate
    {
        private class StatefulListenerParameters
            : GenericListenerParameters,
              IStatefulServiceHostGenericListenerReplicaTemplateParameters,
              IStatefulServiceHostGenericListenerReplicaTemplateConfigurator
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
                dependencies =>
                {
                    dependencies.Add(new ServiceDescriptor(typeof(IReliableStateManager), service.GetReliableStateManager()));
                });

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateFactory(parameters);

            return new ServiceReplicaListener(context => factory(service), parameters.EndpointName, parameters.ListenerOnSecondary);
        }
    }
}