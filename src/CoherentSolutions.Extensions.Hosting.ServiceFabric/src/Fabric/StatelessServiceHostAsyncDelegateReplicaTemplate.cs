namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostAsyncDelegateReplicaTemplate
        : ServiceHostAsyncDelegateReplicaTemplate<
              IStatelessService,
              IStatelessServiceHostAsyncDelegateReplicaTemplateParameters,
              IStatelessServiceHostAsyncDelegateReplicaTemplateConfigurator,
              IServiceHostAsyncDelegate>,
          IStatelessServiceHostAsyncDelegateReplicaTemplate
    {
        private class StatelessAsyncDelegateParameters
            : DelegateParameters,
              IStatelessServiceHostAsyncDelegateReplicaTemplateParameters,
              IStatelessServiceHostAsyncDelegateReplicaTemplateConfigurator
        {
        }

        public override IServiceHostAsyncDelegate Activate(
            IStatelessService service)
        {
            var parameters = new StatelessAsyncDelegateParameters();

            this.UpstreamConfiguration(parameters);

            var func = this.CreateFunc(service, parameters);

            return func();
        }
    }
}