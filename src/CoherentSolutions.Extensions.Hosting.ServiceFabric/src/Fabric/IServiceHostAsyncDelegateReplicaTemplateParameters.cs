using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAsyncDelegateReplicaTemplateParameters : IConfigurableObjectDependenciesParameters
    {
        Delegate Delegate { get; }
    }
}