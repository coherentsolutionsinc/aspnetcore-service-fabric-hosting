using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderEventSourceReplicationParameters<in TReplicableTemplate, out TReplicator>
    {
        Func<TReplicableTemplate, TReplicator> EventSourceReplicatorFunc { get; }
    }
}