namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostDelegateReplicaTemplate
        : ServiceHostDelegateReplicaTemplate<
              IStatelessService,
              IStatelessServiceHostDelegateReplicaTemplateParameters,
              IStatelessServiceHostDelegateReplicaTemplateConfigurator,
              IServiceHostDelegateInvoker>,
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
                this.Event = StatelessServiceLifecycleEvent.OnRunAfterListenersAreOpened;
            }

            public void UseEvent(
                StatelessServiceLifecycleEvent @event)
            {
                this.Event = @event;
            }
        }

        public override IServiceHostDelegateInvoker Activate(
            IStatelessService service)
        {
            var parameters = new StatelessDelegateParameters();

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateFunc(service, parameters);

            return factory();
        }
    }
}