using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderAspNetCoreListenerConfigurator<TAspNetCoreReplicaTemplate>
        : IConfigurableObjectWebHostConfigurator
        where TAspNetCoreReplicaTemplate : IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
    {
        void UseAspNetCoreListenerReplicaTemplate(
            Func<TAspNetCoreReplicaTemplate> factoryFunc);

        void DefineAspNetCoreListener(
            Action<TAspNetCoreReplicaTemplate> defineAction);
    }
}