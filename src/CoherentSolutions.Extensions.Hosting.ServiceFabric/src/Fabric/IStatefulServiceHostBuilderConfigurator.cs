﻿using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostBuilderConfigurator
        : IServiceHostBuilderConfigurator,
          IServiceHostBuilderDelegateConfigurator<IStatefulServiceHostDelegateReplicaTemplate>,
          IServiceHostBuilderDelegateReplicationConfigurator<IStatefulServiceHostDelegateReplicableTemplate, IStatefulServiceHostDelegateReplicator>,
          IServiceHostBuilderAspNetCoreListenerConfigurator<IStatefulServiceHostAspNetCoreListenerReplicaTemplate>,
          IServiceHostBuilderRemotingListenerConfigurator<IStatefulServiceHostRemotingListenerReplicaTemplate>,
          IServiceHostBuilderListenerReplicationConfigurator<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator>
    {
        void UseRuntimeRegistrant(
            Func<IStatefulServiceRuntimeRegistrant> factoryFunc);
    }
}