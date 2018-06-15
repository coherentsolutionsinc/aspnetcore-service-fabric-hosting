using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderRemotingListenerConfigurator<TReplicaTemplate>
        where TReplicaTemplate : IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>
    {
        void UseRemotingListenerReplicaTemplate(
            Func<TReplicaTemplate> factoryFunc);

        void DefineRemotingListener(
            Action<TReplicaTemplate> declareAction);
    }
}