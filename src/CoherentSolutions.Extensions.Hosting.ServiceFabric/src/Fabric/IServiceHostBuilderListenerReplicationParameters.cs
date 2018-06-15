using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderListenerReplicationParameters<in TReplicaTemplate, out TReplicator>
    {
        Func<TReplicaTemplate, TReplicator> ListenerReplicatorFunc { get; }
    }
}