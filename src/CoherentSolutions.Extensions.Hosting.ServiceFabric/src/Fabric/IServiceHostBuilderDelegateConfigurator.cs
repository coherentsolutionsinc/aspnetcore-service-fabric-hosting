using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderDelegateConfigurator<TReplicaTemplate>
        where TReplicaTemplate : IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>
    {
        void UseDelegateReplicaTemplate(
            Func<TReplicaTemplate> factoryFunc);

        void DefineDelegate(
            Action<TReplicaTemplate> defineAction);
    }
}