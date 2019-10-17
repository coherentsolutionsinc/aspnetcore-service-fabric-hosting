using System;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions;

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

            public void UseRuntimeRegistrant(
                Func<IStatelessServiceRuntimeRegistrant> factoryFunc)
            {
                this.RuntimeRegistrantFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
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