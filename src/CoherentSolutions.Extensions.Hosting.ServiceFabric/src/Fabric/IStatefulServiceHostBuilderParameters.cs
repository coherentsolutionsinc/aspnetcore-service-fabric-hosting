using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderEventSourceParameters<IStatefulServiceHostEventSourceReplicaTemplate>,
          IServiceHostBuilderEventSourceReplicationParameters<IStatefulServiceHostEventSourceReplicableTemplate, IStatefulServiceHostEventSourceReplicator>,
          IServiceHostBuilderDelegateParameters<IStatefulServiceHostDelegateReplicaTemplate>,
          IServiceHostBuilderDelegateReplicationParameters<IStatefulServiceHostDelegateReplicableTemplate, IStatefulServiceHostDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerParameters<IStatefulServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
        Func<IStatefulServiceRuntimeRegistrant> RuntimeRegistrantFunc { get; }
    }
}