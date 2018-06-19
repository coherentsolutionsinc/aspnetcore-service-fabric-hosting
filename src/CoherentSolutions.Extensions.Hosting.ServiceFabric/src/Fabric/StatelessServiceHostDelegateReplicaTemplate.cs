namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostDelegateReplicaTemplate
        : ServiceHostDelegateReplicaTemplate<
              IStatelessService,
              IStatelessServiceHostDelegateReplicaTemplateParameters,
              IStatelessServiceHostDelegateReplicaTemplateConfigurator,
              IServiceHostDelegate>,
          IStatelessServiceHostDelegateReplicaTemplate
    {
        private class StatelessAsyncDelegateParameters
            : DelegateParameters,
              IStatelessServiceHostDelegateReplicaTemplateParameters,
              IStatelessServiceHostDelegateReplicaTemplateConfigurator
        {
        }

        public override IServiceHostDelegate Activate(
            IStatelessService service)
        {
            var parameters = new StatelessAsyncDelegateParameters();

            this.UpstreamConfiguration(parameters);

            var func = this.CreateFunc(service, parameters);

            return func();
        }
    }
}