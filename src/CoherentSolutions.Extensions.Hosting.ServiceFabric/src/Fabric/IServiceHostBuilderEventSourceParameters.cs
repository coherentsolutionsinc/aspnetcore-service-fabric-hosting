using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderEventSourceParameters<out TReplicaTemplate>
        where TReplicaTemplate : IServiceHostEventSourceReplicaTemplate<IServiceHostEventSourceReplicaTemplateConfigurator>
    {
        Func<TReplicaTemplate> EventSourceReplicaTemplateFunc { get; }
    }
}