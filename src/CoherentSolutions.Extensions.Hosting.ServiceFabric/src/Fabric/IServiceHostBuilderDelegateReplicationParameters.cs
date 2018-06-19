using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderDelegateReplicationParameters<in TReplicaTemplate, out TReplicator>
    {
        Func<TReplicaTemplate, TReplicator> AsyncDelegateReplicatorFunc { get; }
    }
}