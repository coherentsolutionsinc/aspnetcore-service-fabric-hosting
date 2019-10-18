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

            public void UseRuntimeRegistrant(
                Func<IStatefulServiceRuntimeRegistrant> factoryFunc)
            {
                this.RuntimeRegistrantFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
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