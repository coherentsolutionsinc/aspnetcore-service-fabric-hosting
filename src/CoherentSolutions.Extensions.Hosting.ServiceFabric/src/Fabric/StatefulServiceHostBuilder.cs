using System;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostBuilder
        : ServiceHostBuilder<
              IStatefulServiceHost,
              IStatefulServiceHostBuilderParameters,
              IStatefulServiceHostBuilderConfigurator,
              IStatefulServiceHostEventSourceReplicableTemplate,
              IStatefulServiceHostEventSourceReplicaTemplate,
              IStatefulServiceHostEventSourceReplicator,
              IStatefulServiceHostDelegateReplicableTemplate,
              IStatefulServiceHostDelegateReplicaTemplate,
              IStatefulServiceHostDelegateReplicator,
              IStatefulServiceHostListenerReplicableTemplate,
              IStatefulServiceHostAspNetCoreListenerReplicaTemplate,
              IStatefulServiceHostRemotingListenerReplicaTemplate,
              IStatefulServiceHostGenericListenerReplicaTemplate,
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
                Func<IStatefulServiceRuntimeRegistrant> factoryFunc)
            {
                this.RuntimeRegistrantFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            private static IStatefulServiceRuntimeRegistrant DefaultRuntimeRegistrant()
            {
                return new StatefulServiceRuntimeRegistrant();
            }

            private static IStatefulServiceHostEventSourceReplicaTemplate DefaultEventSourceReplicaTemplateFunc()
            {
                return new StatefulServiceHostEventSourceReplicaTemplate();
            }

            private static IStatefulServiceHostEventSourceReplicator DefaultEventSourceReplicatorFunc(
                IStatefulServiceHostEventSourceReplicableTemplate template)
            {
                return new StatefulServiceHostEventSourceReplicator(template);
            }

            private static IStatefulServiceHostDelegateReplicaTemplate DefaultDelegateReplicaTemplateFunc()
            {
                return new StatefulServiceHostDelegateReplicaTemplate();
            }

            private static IStatefulServiceHostDelegateReplicator DefaultDelegateReplicatorFunc(
                IStatefulServiceHostDelegateReplicableTemplate template)
            {
                return new StatefulServiceHostDelegateReplicator(template);
            }

            private static IStatefulServiceHostAspNetCoreListenerReplicaTemplate DefaultAspNetCoreListenerReplicaTemplateFunc()
            {
                return new StatefulServiceHostAspNetCoreListenerReplicaTemplate();
            }

            private static IStatefulServiceHostRemotingListenerReplicaTemplate DefaultRemotingListenerReplicaTemplateFunc()
            {
                return new StatefulServiceHostRemotingListenerReplicaTemplate();
            }

            private static IStatefulServiceHostGenericListenerReplicaTemplate DefaultGenericListenerReplicaTemplateFunc()
            {
                return new StatefulServiceHostGenericListenerReplicaTemplate();
            }

            private static IStatefulServiceHostListenerReplicator DefaultListenerReplicatorFunc(
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
                compilation.EventSourceReplicator,
                compilation.DelegateReplicators,
                compilation.ListenerReplicators);
        }
    }
}