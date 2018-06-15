using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderAspNetCoreListenerParameters<out TReplicaTemplate>
        where TReplicaTemplate : IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
    {
        Func<TReplicaTemplate> AspNetCoreListenerReplicaTemplateFunc { get; }
    }
}