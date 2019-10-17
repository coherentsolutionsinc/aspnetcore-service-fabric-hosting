using System;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostDelegateReplicaTemplate
        : ServiceHostDelegateReplicaTemplate<
              IStatelessService,
              IStatelessServiceHostDelegateReplicaTemplateParameters,
              IStatelessServiceHostDelegateReplicaTemplateConfigurator,
              StatelessServiceDelegate>,
          IStatelessServiceHostDelegateReplicaTemplate
    {
        private class StatelessDelegateParameters
            : DelegateParameters,
              IStatelessServiceHostDelegateReplicaTemplateParameters,
              IStatelessServiceHostDelegateReplicaTemplateConfigurator
        {
            public StatelessServiceLifecycleEvent Event { get; private set; }

            public StatelessDelegateParameters()
            {
                this.Event = StatelessServiceLifecycleEvent.OnRun;
            }

            public void UseEvent(
                StatelessServiceLifecycleEvent @event)
            {
                this.Event = @event;
            }
        }

        public override StatelessServiceDelegate Activate(
            IStatelessService service)
        {
            var parameters = new StatelessDelegateParameters();

            parameters.ConfigureDependencies(
                dependencies =>
                {
                    // Include specific context registrants
                    dependencies.AddTransient<IServiceDelegateInvocationContextRegistrant, StatelessServiceDelegateInvocationContextRegistrant>();
                });

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateFactory(parameters);

            return new StatelessServiceDelegate(parameters.Event, parameters.Delegate, () => factory(service));
        }
    }
}