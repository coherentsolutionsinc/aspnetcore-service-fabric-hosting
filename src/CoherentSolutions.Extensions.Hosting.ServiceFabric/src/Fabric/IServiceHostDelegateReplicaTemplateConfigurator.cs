using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateReplicaTemplateConfigurator : IConfigurableObjectDependenciesConfigurator
    {
        void UseDelegateInvoker(
            Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker> factoryFunc);

        void UseDelegate(
            Delegate @delegate);
    }
}