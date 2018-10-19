using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceReplicaTemplateConfigurator
    {
        void UseImplementation<TImplementation>(
            Func<TImplementation> factoryFunc)
            where TImplementation : IServiceEventSource;
    }
}