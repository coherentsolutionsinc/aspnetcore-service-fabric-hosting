using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderAsyncDelegateReplicationParameters<in TReplicaTemplate, out TReplicator>
    {
        Func<TReplicaTemplate, TReplicator> AsyncDelegateReplicatorFunc { get; }
    }
}