using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderEventSourceReplicationConfigurator<out TReplicaTemplate, in TReplicator>
    {
        void UseEventSourceReplicator(
            Func<TReplicaTemplate, TReplicator> factoryFunc);
    }
}