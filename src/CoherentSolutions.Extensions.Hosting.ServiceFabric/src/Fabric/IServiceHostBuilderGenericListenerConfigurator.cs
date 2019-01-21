using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderGenericListenerConfigurator<TReplicaTemplate>
        where TReplicaTemplate : IServiceHostGenericListenerReplicaTemplate<IServiceHostGenericListenerReplicaTemplateConfigurator>
    {
        void UseGenericListenerReplicaTemplate(
            Func<TReplicaTemplate> factoryFunc);

        void DefineGenericListener(
            Action<TReplicaTemplate> defineAction);
    }
}