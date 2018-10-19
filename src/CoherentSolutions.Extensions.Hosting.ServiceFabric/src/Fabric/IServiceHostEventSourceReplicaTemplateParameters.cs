using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceReplicaTemplateParameters
        : IConfigurableObjectDependenciesParameters
    {
        Func<IServiceProvider, IServiceEventSource> ImplementationFunc { get; }
    }
}