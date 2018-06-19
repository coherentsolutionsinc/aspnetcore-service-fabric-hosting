namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostBuilder
        : ServiceHostBuilder<
              IStatefulServiceHost,
              IStatefulServiceHostBuilderParameters,
              IStatefulServiceHostBuilderConfigurator,
              IStatefulServiceHostDelegateReplicableTemplate,
              IStatefulServiceHostDelegateReplicaTemplate,
              IStatefulServiceHostDelegateReplicator,
              IStatefulServiceHostListenerReplicableTemplate,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplate,
              IStatefulServiceHostRemotingListenerReplicaTemplate,
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
                this.UseAsyncDelegateReplicaTemplate(DefaultAsyncDelegateReplicaTemplate);
                this.UseAsyncDelegateReplicator(DefaultAsyncDelegateReplicatorFactory);
                this.UseAspNetCoreListenerReplicaTemplate(DefaultAspNetCoreListenerReplicaTemplate);
                this.UseRemotingListenerReplicaTemplate(DefaultRemotingListenerReplicaTemplate);
                this.UseListenerReplicator(DefaultListenerReplicatorFactory);
            }

            private static IStatefulServiceHostDelegateReplicaTemplate DefaultAsyncDelegateReplicaTemplate()
            {
                return new StatefulServiceHostDelegateReplicaTemplate();
            }

            private static IStatefulServiceHostDelegateReplicator DefaultAsyncDelegateReplicatorFactory(
                IStatefulServiceHostDelegateReplicableTemplate template)
            {
                return new StatefulServiceHostDelegateReplicator(template);
            }

            private static IStatefulServiceHostAspNetCoreListenerReplicaTemplate DefaultAspNetCoreListenerReplicaTemplate()
            {
                return new StatefulServiceHostAspNetCoreListenerReplicaTemplate();
            }

            private static IStatefulServiceHostRemotingListenerReplicaTemplate DefaultRemotingListenerReplicaTemplate()
            {
                return new StatefulServiceHostRemotingListenerReplicaTemplate();
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

            var compilation = this.CompileParameters(parameters);

            return new StatefulServiceHost(
                parameters.ServiceTypeName, 
                compilation.DelegateReplicators,
                compilation.ListenerReplicators);
        }
    }
}