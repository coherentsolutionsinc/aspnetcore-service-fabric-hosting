using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderListenerReplicationConfigurator<out TReplicaTemplate, in TReplicator>
    {
        void UseListenerReplicator(
            Func<TReplicaTemplate, TReplicator> factoryFunc);
    }
}