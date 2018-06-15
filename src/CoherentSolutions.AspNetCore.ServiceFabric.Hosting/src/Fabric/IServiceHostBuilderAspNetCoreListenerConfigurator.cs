using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderAspNetCoreListenerConfigurator<TReplicaTemplate>
        where TReplicaTemplate : IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
    {
        void UseAspNetCoreListenerReplicaTemplate(
            Func<TReplicaTemplate> factoryFunc);

        void DefineAspNetCoreListener(
            Action<TReplicaTemplate> defineAction);
    }
}