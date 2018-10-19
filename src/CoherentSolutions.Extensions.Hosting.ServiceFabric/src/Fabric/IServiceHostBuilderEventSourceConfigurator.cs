using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderEventSourceConfigurator<in TReplicaTemplate>
        where TReplicaTemplate : IServiceHostEventSourceReplicaTemplate<IServiceHostEventSourceReplicaTemplateConfigurator>
    {
        void UseEventSourceReplicaTemplate(
            Func<TReplicaTemplate> factoryFunc);
    }
}