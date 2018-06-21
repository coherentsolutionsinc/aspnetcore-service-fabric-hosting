using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateReplicaTemplateParameters : IConfigurableObjectDependenciesParameters
    {
        Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker> DelegateInvokerFunc { get; }

        Delegate Delegate { get; }
    }
}