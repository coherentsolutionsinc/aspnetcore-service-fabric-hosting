using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderAspNetCoreListenerParameters<out TReplicaTemplate>
        where TReplicaTemplate : IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
    {
        Func<TReplicaTemplate> AspNetCoreListenerReplicaTemplateFunc { get; }
    }
}