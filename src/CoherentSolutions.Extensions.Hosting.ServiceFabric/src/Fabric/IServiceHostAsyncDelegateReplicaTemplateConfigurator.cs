using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAsyncDelegateReplicaTemplateConfigurator : IConfigurableObjectDependenciesConfigurator
    {
        void UseDelegate(
            Delegate @delegate);
    }
}