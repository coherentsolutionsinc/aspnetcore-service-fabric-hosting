using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderRuntimeParameters<IStatefulServiceRuntimeRegistrant>,
          IServiceHostBuilderEventSourceParameters<IStatefulServiceHostEventSourceReplicaTemplate>,
          IServiceHostBuilderEventSourceReplicationParameters<IStatefulServiceHostEventSourceReplicableTemplate, IStatefulServiceHostEventSourceReplicator>,
          IServiceHostBuilderDelegateParameters<IStatefulServiceHostDelegateReplicaTemplate>,
          IServiceHostBuilderDelegateReplicationParameters<IStatefulServiceHostDelegateReplicableTemplate, IStatefulServiceHostDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerParameters<IStatefulServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderGenericListenerParameters<IStatefulServiceHostGenericListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
    }
}