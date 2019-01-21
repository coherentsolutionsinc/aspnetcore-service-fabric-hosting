using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostBuilder
        : ServiceHostBuilder<
              IStatelessServiceHost,
              IStatelessServiceHostBuilderParameters,
              IStatelessServiceHostBuilderConfigurator,
              IStatelessServiceHostEventSourceReplicableTemplate,
              IStatelessServiceHostEventSourceReplicaTemplate,
              IStatelessServiceHostEventSourceReplicator,
              IStatelessServiceHostDelegateReplicableTemplate,
              IStatelessServiceHostDelegateReplicaTemplate,
              IStatelessServiceHostDelegateReplicator,
              IStatelessServiceHostListenerReplicableTemplate,
              IStatelessServiceHostAspNetCoreListenerReplicaTemplate,
              IStatelessServiceHostRemotingListenerReplicaTemplate,
              IStatelessServiceHostGenericListenerReplicaTemplate,
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

                this.UseEventSourceReplicaTemplate(DefaultEventSourceReplicaTemplateFunc);
                this.UseEventSourceReplicator(DefaultEventSourceReplicatorFunc);
                this.UseDelegateReplicaTemplate(DefaultDelegateReplicaTemplateFunc);
                this.UseDelegateReplicator(DefaultDelegateReplicatorFunc);
                this.UseAspNetCoreListenerReplicaTemplate(DefaultAspNetCoreListenerReplicaTemplateFunc);
                this.UseRemotingListenerReplicaTemplate(DefaultRemotingListenerReplicaTemplateFunc);
                this.UseGenericListenerReplicaTemplate(DefaultGenericListenerReplicaTemplateFunc);
                this.UseListenerReplicator(DefaultListenerReplicatorFunc);
            }

            public void UseRuntimeRegistrant(
                Func<IStatelessServiceRuntimeRegistrant> factoryFunc)
            {
                this.RuntimeRegistrantFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            private static IStatelessServiceHostEventSourceReplicaTemplate DefaultEventSourceReplicaTemplateFunc()
            {
                return new StatelessServiceHostEventSourceReplicaTemplate();
            }

            private static IStatelessServiceRuntimeRegistrant DefaultRuntimeRegistrant()
            {
                return new StatelessServiceRuntimeRegistrant();
            }

            private static IStatelessServiceHostEventSourceReplicator DefaultEventSourceReplicatorFunc(
                IStatelessServiceHostEventSourceReplicableTemplate replicableTemplate)
            {
                return new StatelessServiceHostEventSourceReplicator(replicableTemplate);
            }

            private static IStatelessServiceHostDelegateReplicaTemplate DefaultDelegateReplicaTemplateFunc()
            {
                return new StatelessServiceHostDelegateReplicaTemplate();
            }

            private static IStatelessServiceHostDelegateReplicator DefaultDelegateReplicatorFunc(
                IStatelessServiceHostDelegateReplicableTemplate template)
            {
                return new StatelessServiceHostDelegateReplicator(template);
            }

            private static IStatelessServiceHostAspNetCoreListenerReplicaTemplate DefaultAspNetCoreListenerReplicaTemplateFunc()
            {
                return new StatelessServiceHostAspNetCoreListenerReplicaTemplate();
            }

            private static IStatelessServiceHostRemotingListenerReplicaTemplate DefaultRemotingListenerReplicaTemplateFunc()
            {
                return new StatelessServiceHostRemotingListenerReplicaTemplate();
            }

            private static IStatelessServiceHostGenericListenerReplicaTemplate DefaultGenericListenerReplicaTemplateFunc()
            {
                return new StatelessServiceHostGenericListenerReplicaTemplate();
            }

            private static IStatelessServiceHostListenerReplicator DefaultListenerReplicatorFunc(
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
                compilation.EventSourceReplicator,
                compilation.DelegateReplicators,
                compilation.ListenerReplicators);
        }
    }
}