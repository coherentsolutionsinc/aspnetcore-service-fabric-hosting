using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderDelegateReplicationConfigurator<out TReplicaTemplate, in TReplicator>
    {
        void UseDelegateReplicator(
            Func<TReplicaTemplate, TReplicator> factoryFunc);
    }
}