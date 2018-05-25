using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderListenerReplicationParameters<in TReplicaTemplate, out TReplicator>
    {
        Func<TReplicaTemplate, TReplicator> ListenerReplicatorFunc { get; }
    }
}