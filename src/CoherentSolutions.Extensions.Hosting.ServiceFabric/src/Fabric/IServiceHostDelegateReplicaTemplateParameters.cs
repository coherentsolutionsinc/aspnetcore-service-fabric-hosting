using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateReplicaTemplateParameters : IConfigurableObjectDependenciesParameters
    {
        Delegate Delegate { get; }
    }
}