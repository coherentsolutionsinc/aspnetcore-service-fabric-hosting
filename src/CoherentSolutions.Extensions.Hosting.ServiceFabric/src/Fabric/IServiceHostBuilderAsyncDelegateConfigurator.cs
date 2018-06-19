using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderAsyncDelegateConfigurator<TReplicaTemplate>
        where TReplicaTemplate : IServiceHostAsyncDelegateReplicaTemplate<IServiceHostAsyncDelegateReplicaTemplateConfigurator>
    {
        void UseAsyncDelegateReplicaTemplate(
            Func<TReplicaTemplate> factoryFunc);

        void DefineAsyncDelegate(
            Action<TReplicaTemplate> defineAction);
    }
}