using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostDelegateReplicaTemplate
        : ServiceHostDelegateReplicaTemplate<
              IStatefulService,
              IStatefulServiceHostDelegateReplicaTemplateParameters,
              IStatefulServiceHostDelegateReplicaTemplateConfigurator,
              IServiceHostDelegateInvoker>,
          IStatefulServiceHostDelegateReplicaTemplate
    {
        private class StatefulDelegateParameters
            : DelegateParameters,
              IStatefulServiceHostDelegateReplicaTemplateParameters,
              IStatefulServiceHostDelegateReplicaTemplateConfigurator
        {
            public StatefulServiceLifecycleEvent Event { get; private set; }

            public StatefulDelegateParameters()
            {
                this.Event = StatefulServiceLifecycleEvent.OnRunAfterListenersAreOpened;
            }

            public void UseEvent(StatefulServiceLifecycleEvent @event)
            {
                this.Event = @event;
            }
        }

        public override IServiceHostDelegateInvoker Activate(
            IStatefulService service)
        {
            var parameters = new StatefulDelegateParameters();

            parameters.ConfigureDependencies(
                dependencies =>
                {
                    dependencies.Add(new ServiceDescriptor(typeof(IReliableStateManager), service.GetReliableStateManager()));
                });

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateFunc(service, parameters);

            return factory();
        }
    }
}