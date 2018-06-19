using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderAsyncDelegateReplicationConfigurator<out TReplicaTemplate, in TReplicator>
    {
        void UseAsyncDelegateReplicator(
            Func<TReplicaTemplate, TReplicator> factoryFunc);
    }
}