using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderAsyncDelegateParameters<out TReplicaTemplate>
        where TReplicaTemplate : IServiceHostAsyncDelegateReplicaTemplate<IServiceHostAsyncDelegateReplicaTemplateConfigurator>
    {
        Func<TReplicaTemplate> AsyncDelegateReplicaTemplateFunc { get; }
    }
}