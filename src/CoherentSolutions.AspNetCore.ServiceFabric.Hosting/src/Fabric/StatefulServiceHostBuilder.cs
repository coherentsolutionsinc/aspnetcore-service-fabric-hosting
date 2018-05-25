namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class StatefulServiceHostBuilder
        : ServiceHostBuilder<
              IStatefulServiceHost,
              IStatefulServiceHostBuilderParameters,
              IStatefulServiceHostBuilderConfigurator,
              IStatefulServiceHostListenerReplicableTemplate,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplate,
              IStatefulServiceHostListenerReplicator>,
          IStatefulServiceHostBuilder
    {
        private class StatefulParameters
            : Parameters,
              IStatefulServiceHostBuilderParameters,
              IStatefulServiceHostBuilderConfigurator
        {
            public StatefulParameters()
            {
                this.UseAspNetCoreListenerReplicaTemplate(DefaultAspNetCoreListenerReplicaTemplate);
                this.UseListenerReplicator(DefaultListenerReplicatorFactory);
            }

            private static IStatefulServiceHostAspNetCoreListenerReplicaTemplate DefaultAspNetCoreListenerReplicaTemplate()
            {
                return new StatefulServiceHostAspNetCoreListenerReplicaTemplate();
            }

            private static IStatefulServiceHostListenerReplicator DefaultListenerReplicatorFactory(
                IStatefulServiceHostListenerReplicableTemplate template)
            {
                return new StatefulServiceHostListenerReplicator(template);
            }
        }

        public override IStatefulServiceHost Build()
        {
            var parameters = new StatefulParameters();

            this.UpstreamConfiguration(parameters);

            var replicators = this.BuildReplicators(parameters);

            return new StatefulServiceHost(parameters.ServiceName, replicators);
        }
    }
}