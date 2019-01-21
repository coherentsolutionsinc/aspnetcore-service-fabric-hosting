using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderGenericListenerParameters<out TReplicaTemplate>
        where TReplicaTemplate : IServiceHostGenericListenerReplicaTemplate<IServiceHostGenericListenerReplicaTemplateConfigurator>
    {
        Func<TReplicaTemplate> GenericListenerReplicaTemplateFunc { get; }
    }
}