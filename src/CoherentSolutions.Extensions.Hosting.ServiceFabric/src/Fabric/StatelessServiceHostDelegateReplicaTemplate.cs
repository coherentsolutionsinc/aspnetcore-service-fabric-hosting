using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostDelegateReplicaTemplate
        : ServiceHostDelegateReplicaTemplate<
              IStatelessService,
              IStatelessServiceHostDelegateReplicaTemplateParameters,
              IStatelessServiceHostDelegateReplicaTemplateConfigurator,
              IStatelessServiceHostDelegateInvoker,
              StatelessServiceDelegate>,
          IStatelessServiceHostDelegateReplicaTemplate
    {
        private class StatelessDelegateParameters
            : DelegateParameters,
              IStatelessServiceHostDelegateReplicaTemplateParameters,
              IStatelessServiceHostDelegateReplicaTemplateConfigurator
        {
            public StatelessServiceLifecycleEvent Event { get; private set; }

            public Func<Delegate, IServiceProvider, IStatelessServiceHostDelegateInvoker> DelegateInvokerFunc { get; private set; }

            public StatelessDelegateParameters()
            {
                this.Event = StatelessServiceLifecycleEvent.OnRunAfterListenersAreOpened;
                this.DelegateInvokerFunc = DefaultDelegateInvokerFunc;
            }

            public void UseEvent(
                StatelessServiceLifecycleEvent @event)
            {
                this.Event = @event;
            }

            public void UseDelegateInvoker(
                Func<Delegate, IServiceProvider, IStatelessServiceHostDelegateInvoker> factoryFunc)
            {
                this.DelegateInvokerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            private static IStatelessServiceHostDelegateInvoker DefaultDelegateInvokerFunc(
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

                return new StatelessServiceHostDelegateInvoker(@delegate, services);
            }
        }

        public override StatelessServiceDelegate Activate(
            IStatelessService service)
        {
            var parameters = new StatelessDelegateParameters();

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateDelegateInvokerFunc(service, parameters);

            return new StatelessServiceDelegate(() => factory(parameters.DelegateInvokerFunc), parameters.Event);
        }
    }
}