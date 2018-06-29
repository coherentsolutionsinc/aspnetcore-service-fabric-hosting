using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostBuilder
        : ServiceHostBuilder<
              IStatelessServiceHost,
              IStatelessServiceHostBuilderParameters,
              IStatelessServiceHostBuilderConfigurator,
              IStatelessServiceHostDelegateReplicableTemplate,
              IStatelessServiceHostDelegateReplicaTemplate,
              IStatelessServiceHostDelegateReplicator,
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
            public Func<IStatelessServiceRuntimeRegistrant> RuntimeRegistrantFunc { get; private set; }

            public StatelessParameters()
            {
                this.RuntimeRegistrantFunc = DefaultRuntimeRegistrant;

                this.UseDelegateReplicaTemplate(DefaultDelegateReplicaTemplate);
                this.UseDelegateReplicator(DefaultDelegateReplicatorFactory);
                this.UseAspNetCoreListenerReplicaTemplate(DefaultAspNetCoreListenerReplicaTemplate);
                this.UseRemotingListenerReplicaTemplate(DefaultRemotingListenerReplicaTemplate);
                this.UseListenerReplicator(DefaultListenerReplicatorFactory);
            }

            public void UseRuntimeRegistrant(
                Func<IStatelessServiceRuntimeRegistrant> factoryFunc)
            {
                this.RuntimeRegistrantFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            private static IStatelessServiceRuntimeRegistrant DefaultRuntimeRegistrant()
            {
                return new StatelessServiceRuntimeRegistrant();
            }

            private static IStatelessServiceHostDelegateReplicaTemplate DefaultDelegateReplicaTemplate()
            {
                return new StatelessServiceHostDelegateReplicaTemplate();
            }

            private static IStatelessServiceHostDelegateReplicator DefaultDelegateReplicatorFactory(
                IStatelessServiceHostDelegateReplicableTemplate template)
            {
                return new StatelessServiceHostDelegateReplicator(template);
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

            var registrant = parameters.RuntimeRegistrantFunc();
            if (registrant == null)
            {
                throw new FactoryProducesNullInstanceException<IStatefulServiceRuntimeRegistrant>();
            }

            var compilation = this.CompileParameters(parameters);

            return new StatelessServiceHost(
                parameters.ServiceTypeName,
                registrant,
                compilation.DelegateReplicators,
                compilation.ListenerReplicators);
        }
    }
}