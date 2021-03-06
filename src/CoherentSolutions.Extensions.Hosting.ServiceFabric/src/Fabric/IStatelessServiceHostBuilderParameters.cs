﻿using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostBuilderParameters
        : IServiceHostBuilderParameters,
          IServiceHostBuilderRuntimeParameters<IStatelessServiceRuntimeRegistrant>,
          IServiceHostBuilderEventSourceParameters<IStatelessServiceHostEventSourceReplicaTemplate>,
          IServiceHostBuilderEventSourceReplicationParameters<IStatelessServiceHostEventSourceReplicableTemplate, IStatelessServiceHostEventSourceReplicator>,
          IServiceHostBuilderDelegateParameters<IStatelessServiceHostDelegateReplicaTemplate>,
          IServiceHostBuilderDelegateReplicationParameters<IStatelessServiceHostDelegateReplicableTemplate, IStatelessServiceHostDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerParameters<IStatelessServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerParameters<IStatelessServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderGenericListenerParameters<IStatelessServiceHostGenericListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationParameters<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator>
    {
    }
}