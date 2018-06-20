using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateReplicaTemplateConfigurator : IConfigurableObjectDependenciesConfigurator
    {
        void UseDelegate(
            Delegate @delegate);

        void UseLifecycleEvent(
            ServiceLifecycleEvent schedule);
    }
}