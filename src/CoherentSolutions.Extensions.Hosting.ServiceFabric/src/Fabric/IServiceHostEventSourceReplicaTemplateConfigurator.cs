using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceReplicaTemplateConfigurator
        : IConfigurableObjectDependenciesConfigurator
    {
        void UseImplementation<TImplementation>(
            Func<IServiceProvider, TImplementation> factoryFunc)
            where TImplementation : IServiceEventSource;
    }
}