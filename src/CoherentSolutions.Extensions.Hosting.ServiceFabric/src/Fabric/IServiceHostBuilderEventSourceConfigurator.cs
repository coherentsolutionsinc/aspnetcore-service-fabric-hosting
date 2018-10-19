using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderEventSourceConfigurator<TReplicaTemplate>
        where TReplicaTemplate : IServiceHostEventSourceReplicaTemplate<IServiceHostEventSourceReplicaTemplateConfigurator>
    {
        void UseEventSourceReplicaTemplate(
            Func<TReplicaTemplate> factoryFunc);

        void SetupEventSource(
            Action<TReplicaTemplate> setupAction);
    }
}