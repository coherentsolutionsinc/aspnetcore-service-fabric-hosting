using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderDelegateParameters<out TReplicaTemplate>
        where TReplicaTemplate : IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>
    {
        Func<TReplicaTemplate> AsyncDelegateReplicaTemplateFunc { get; }
    }
}