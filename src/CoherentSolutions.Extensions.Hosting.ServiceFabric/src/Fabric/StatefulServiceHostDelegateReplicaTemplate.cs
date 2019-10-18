using System;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostDelegateReplicaTemplate
        : ServiceHostDelegateReplicaTemplate<
              IStatefulService,
              IStatefulServiceHostDelegateReplicaTemplateParameters,
              IStatefulServiceHostDelegateReplicaTemplateConfigurator,
              StatefulServiceDelegate>,
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
                this.Event = StatefulServiceLifecycleEvent.OnRun;
            }

            public void UseEvent(
                StatefulServiceLifecycleEvent @event)
            {
                this.Event = @event;
            }
        }

        public override StatefulServiceDelegate Activate(
            IStatefulService service)
        {
            var parameters = new StatefulDelegateParameters();

            parameters.ConfigureDependencies(
                dependencies =>
                {
                    // Include reliable state manager
                    dependencies.Add(new ServiceDescriptor(typeof(IReliableStateManager), service.GetReliableStateManager()));

                    // Include specific context registrants
                    dependencies.AddTransient<IServiceDelegateInvocationContextRegistrant, StatefulServiceDelegateInvocationContextRegistrant>();
                });

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateFactory(parameters);

            return new StatefulServiceDelegate(parameters.Event, parameters.Delegate, () => factory(service));
        }
    }
}