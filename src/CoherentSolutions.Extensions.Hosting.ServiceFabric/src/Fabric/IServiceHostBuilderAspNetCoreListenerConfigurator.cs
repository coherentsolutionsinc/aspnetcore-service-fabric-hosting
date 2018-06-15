using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
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