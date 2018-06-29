using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;

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
            public Func<IStatefulServiceRuntimeRegistrant> RuntimeRegistrantFunc { get; private set; }

            public StatefulParameters()
            {
                this.RuntimeRegistrantFunc = DefaultRuntimeRegistrant;

                this.UseDelegateReplicaTemplate(DefaultDelegateReplicaTemplate);
                this.UseDelegateReplicator(DefaultDelegateReplicatorFactory);
                this.UseAspNetCoreListenerReplicaTemplate(DefaultAspNetCoreListenerReplicaTemplate);
                this.UseRemotingListenerReplicaTemplate(DefaultRemotingListenerReplicaTemplate);
                this.UseListenerReplicator(DefaultListenerReplicatorFactory);
            }

            public void UseRuntimeRegistrant(
                Func<IStatefulServiceRuntimeRegistrant> factoryFunc)
            {
                this.RuntimeRegistrantFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            private static IStatefulServiceRuntimeRegistrant DefaultRuntimeRegistrant()
            {
                return new StatefulServiceRuntimeRegistrant();
            }

            private static IStatefulServiceHostDelegateReplicaTemplate DefaultDelegateReplicaTemplate()
            {
                return new StatefulServiceHostDelegateReplicaTemplate();
            }

            private static IStatefulServiceHostDelegateReplicator DefaultDelegateReplicatorFactory(
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

            var registrant = parameters.RuntimeRegistrantFunc();
            if (registrant == null)
            {
                throw new FactoryProducesNullInstanceException<IStatefulServiceRuntimeRegistrant>();
            }

            var compilation = this.CompileParameters(parameters);

            return new StatefulServiceHost(
                parameters.ServiceTypeName,
                registrant,
                compilation.DelegateReplicators,
                compilation.ListenerReplicators);
        }
    }
}