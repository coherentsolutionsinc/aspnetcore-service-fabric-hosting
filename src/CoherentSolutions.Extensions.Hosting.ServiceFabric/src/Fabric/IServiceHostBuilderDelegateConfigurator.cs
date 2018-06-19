using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderDelegateConfigurator<TReplicaTemplate>
        where TReplicaTemplate : IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>
    {
        void UseAsyncDelegateReplicaTemplate(
            Func<TReplicaTemplate> factoryFunc);

        void DefineAsyncDelegate(
            Action<TReplicaTemplate> defineAction);
    }
}