namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class StatelessServiceHostBuilder
        : ServiceHostBuilder<
              IStatelessServiceHost,
              IStatelessServiceHostBuilderParameters,
              IStatelessServiceHostBuilderConfigurator,
              IStatelessServiceHostListenerReplicableTemplate,
              IStatelessServiceHostAspNetCoreListenerReplicaTemplate,
              IStatelessServiceHostRemotingListenerReplicaTemplate,
              IStatelessServiceHostListenerReplicator>,
          IStatelessServiceHostBuilder
    {
        private class StatelessParameters
            : Parameters,
              IStatelessServiceHostBuilderParameters,
              IStatelessServiceHostBuilderConfigurator
        {
            public StatelessParameters()
            {
                this.UseAspNetCoreListenerReplicaTemplate(DefaultAspNetCoreListenerReplicaTemplate);
                this.UseRemotingListenerReplicaTemplate(DefaultRemotingListenerReplicaTemplate);
                this.UseListenerReplicator(DefaultListenerReplicatorFactory);
            }

            private static IStatelessServiceHostAspNetCoreListenerReplicaTemplate DefaultAspNetCoreListenerReplicaTemplate()
            {
                return new StatelessServiceHostAspNetCoreListenerReplicaTemplate();
            }

            private static IStatelessServiceHostRemotingListenerReplicaTemplate DefaultRemotingListenerReplicaTemplate()
            {
                return new StatelessServiceHostRemotingListenerReplicaTemplate();
            }

            private static IStatelessServiceHostListenerReplicator DefaultListenerReplicatorFactory(
                IStatelessServiceHostListenerReplicableTemplate template)
            {
                return new StatelessServiceHostListenerReplicator(template);
            }
        }

        public override IStatelessServiceHost Build()
        {
            var parameters = new StatelessParameters();

            this.UpstreamConfiguration(parameters);

            var replicators = this.BuildReplicators(parameters);

            return new StatelessServiceHost(parameters.ServiceTypeName, replicators);
        }
    }
}