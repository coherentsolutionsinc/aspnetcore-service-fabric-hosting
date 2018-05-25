using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderListenerReplicationConfigurator<out TReplicaTemplate, in TReplicator>
    {
        void UseListenerReplicator(
            Func<TReplicaTemplate, TReplicator> factoryFunc);
    }
}