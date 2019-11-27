using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostBuilder
        : ServiceHostBuilder<
              IStatelessServiceHost,
              IStatelessServiceHostBuilderParameters,
              IStatelessServiceHostBuilderConfigurator,
              IStatelessServiceRuntimeRegistrant,
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
            public StatelessParameters()
            {
                this.UseRuntimeRegistrant(DefaultRuntimeRegistrant);
                this.UseEventSourceReplicaTemplate(DefaultEventSourceReplicaTemplateFunc);
                this.UseEventSourceReplicator(DefaultEventSourceReplicatorFunc);
                this.UseDelegateReplicaTemplate(DefaultDelegateReplicaTemplateFunc);
                this.UseDelegateReplicator(DefaultDelegateReplicatorFunc);
                this.UseAspNetCoreListenerReplicaTemplate(DefaultAspNetCoreListenerReplicaTemplateFunc);
                this.UseRemotingListenerReplicaTemplate(DefaultRemotingListenerReplicaTemplateFunc);
                this.UseGenericListenerReplicaTemplate(DefaultGenericListenerReplicaTemplateFunc);
                this.UseListenerReplicator(DefaultListenerReplicatorFunc);
            }

            private static IStatelessServiceRuntimeRegistrant DefaultRuntimeRegistrant(
                IServiceProvider provider)
            {
                return new StatelessServiceRuntimeRegistrant();
            }

            private static IStatelessServiceHostEventSourceReplicaTemplate DefaultEventSourceReplicaTemplateFunc()
            {
                return new StatelessServiceHostEventSourceReplicaTemplate();
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

            var compilation = this.CompileParameters(parameters);

            return new StatelessServiceHost(
                parameters.ServiceTypeName,
                compilation.RuntimeRegistrant,
                compilation.EventSourceReplicator,
                compilation.DelegateReplicators,
                compilation.ListenerReplicators);
        }
    }
}