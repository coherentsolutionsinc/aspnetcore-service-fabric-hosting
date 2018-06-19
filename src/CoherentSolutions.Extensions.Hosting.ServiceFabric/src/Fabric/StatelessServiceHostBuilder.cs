namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostBuilder
        : ServiceHostBuilder<
              IStatelessServiceHost,
              IStatelessServiceHostBuilderParameters,
              IStatelessServiceHostBuilderConfigurator,
              IStatelessServiceHostAsyncDelegateReplicableTemplate,
              IStatelessServiceHostAsyncDelegateReplicaTemplate,
              IStatelessServiceHostAsyncDelegateReplicator,
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
                this.UseAsyncDelegateReplicaTemplate(DefaultAsyncDelegateReplicaTemplate);
                this.UseAsyncDelegateReplicator(DefaultAsyncDelegateReplicatorFactory);
                this.UseAspNetCoreListenerReplicaTemplate(DefaultAspNetCoreListenerReplicaTemplate);
                this.UseRemotingListenerReplicaTemplate(DefaultRemotingListenerReplicaTemplate);
                this.UseListenerReplicator(DefaultListenerReplicatorFactory);
            }

            private static IStatelessServiceHostAsyncDelegateReplicaTemplate DefaultAsyncDelegateReplicaTemplate()
            {
                return new StatelessServiceHostAsyncDelegateReplicaTemplate();
            }

            private static IStatelessServiceHostAsyncDelegateReplicator DefaultAsyncDelegateReplicatorFactory(
                IStatelessServiceHostAsyncDelegateReplicableTemplate template)
            {
                return new StatelessServiceHostAsyncDelegateReplicator(template);
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

            var compilation = this.CompileParameters(parameters);

            return new StatelessServiceHost(
                parameters.ServiceTypeName, 
                compilation.DelegateReplicators,
                compilation.ListenerReplicators);
        }
    }
}