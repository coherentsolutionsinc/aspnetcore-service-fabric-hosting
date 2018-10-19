using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderEventSourceConfigurator<in TReplicator>
        where TReplicator : IServiceHostEventSourceReplicator
    {
        void UseEventSourceReplicator(
            Func<TReplicator> factoryFunc);
    }
}