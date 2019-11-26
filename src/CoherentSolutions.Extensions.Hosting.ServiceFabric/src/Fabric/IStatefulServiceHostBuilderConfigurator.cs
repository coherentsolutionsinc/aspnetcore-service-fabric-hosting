using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostBuilderConfigurator
        : IServiceHostBuilderConfigurator,
          IServiceHostBuilderRuntimeConfigurator<IStatefulServiceRuntimeRegistrant>,
          IServiceHostBuilderEventSourceConfigurator<IStatefulServiceHostEventSourceReplicaTemplate>,
          IServiceHostBuilderEventSourceReplicationConfigurator<IStatefulServiceHostEventSourceReplicableTemplate, IStatefulServiceHostEventSourceReplicator>,
          IServiceHostBuilderDelegateConfigurator<IStatefulServiceHostDelegateReplicaTemplate>,
          IServiceHostBuilderDelegateReplicationConfigurator<IStatefulServiceHostDelegateReplicableTemplate, IStatefulServiceHostDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerConfigurator<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerConfigurator<IStatefulServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderGenericListenerConfigurator<IStatefulServiceHostGenericListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationConfigurator<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
    }
}