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
        private class StatelessAsyncDelegateParameters
            : DelegateParameters,
              IStatelessServiceHostDelegateReplicaTemplateParameters,
              IStatelessServiceHostDelegateReplicaTemplateConfigurator
        {
        }

        public override IServiceHostDelegateInvoker Activate(
            IStatelessService service)
        {
            var parameters = new StatelessAsyncDelegateParameters();

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateFunc(service, parameters);

            return factory();
        }
    }
}