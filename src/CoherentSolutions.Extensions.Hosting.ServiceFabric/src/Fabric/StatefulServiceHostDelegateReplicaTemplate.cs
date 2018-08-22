using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostDelegateReplicaTemplate
        : ServiceHostDelegateReplicaTemplate<
              IStatefulService,
              IStatefulServiceHostDelegateReplicaTemplateParameters,
              IStatefulServiceHostDelegateReplicaTemplateConfigurator,
              IStatefulServiceHostDelegateInvoker,
              StatefulServiceDelegate>,
          IStatefulServiceHostDelegateReplicaTemplate
    {
        private class StatefulDelegateParameters
            : DelegateParameters,
              IStatefulServiceHostDelegateReplicaTemplateParameters,
              IStatefulServiceHostDelegateReplicaTemplateConfigurator
        {
            public StatefulServiceLifecycleEvent Event { get; private set; }

            public Func<Delegate, IServiceProvider, IStatefulServiceHostDelegateInvoker> DelegateInvokerFunc { get; private set; }

            public StatefulDelegateParameters()
            {
                this.Event = StatefulServiceLifecycleEvent.OnRunAfterListenersAreOpened;
                this.DelegateInvokerFunc = DefaultDelegateInvokerFunc;
            }

            public void UseEvent(
                StatefulServiceLifecycleEvent @event)
            {
                this.Event = @event;
            }

            public void UseDelegateInvoker(
                Func<Delegate, IServiceProvider, IStatefulServiceHostDelegateInvoker> factoryFunc)
            {
                this.DelegateInvokerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            private static IStatefulServiceHostDelegateInvoker DefaultDelegateInvokerFunc(
                Delegate @delegate,
                IServiceProvider services)
            {
                if (@delegate == null)
                {
                    throw new ArgumentNullException(nameof(@delegate));
                }

                if (services == null)
                {
                    throw new ArgumentNullException(nameof(services));
                }

                return new StatefulServiceHostDelegateInvoker(@delegate, services);
            }
        }

        public override StatefulServiceDelegate Activate(
            IStatefulService service)
        {
            var parameters = new StatefulDelegateParameters();

            parameters.ConfigureDependencies(
                dependencies =>
                {
                    dependencies.Add(new ServiceDescriptor(typeof(IReliableStateManager), service.GetReliableStateManager()));
                });

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateDelegateInvokerFunc(service, parameters);

            return new StatefulServiceDelegate(() => factory(parameters.DelegateInvokerFunc), parameters.Event);
        }
    }
}