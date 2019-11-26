using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostBuilderConfigurator
        : IServiceHostBuilderConfigurator,
          IServiceHostBuilderRuntimeConfigurator<IStatelessServiceRuntimeRegistrant>,
          IServiceHostBuilderEventSourceConfigurator<IStatelessServiceHostEventSourceReplicaTemplate>,
          IServiceHostBuilderEventSourceReplicationConfigurator<IStatelessServiceHostEventSourceReplicableTemplate, IStatelessServiceHostEventSourceReplicator>,
          IServiceHostBuilderDelegateConfigurator<IStatelessServiceHostDelegateReplicaTemplate>,
          IServiceHostBuilderDelegateReplicationConfigurator<IStatelessServiceHostDelegateReplicableTemplate, IStatelessServiceHostDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerConfigurator<IStatelessServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerConfigurator<IStatelessServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderGenericListenerConfigurator<IStatelessServiceHostGenericListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationConfigurator<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator>

    {
    }
}