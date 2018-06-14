using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderRemotingListenerConfigurator<TRemotingReplicaTemplate>
        where TRemotingReplicaTemplate : IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>
    {
        void UseRemotingListenerReplicaTemplate(
            Func<TRemotingReplicaTemplate> factoryFunc);

        void DefineRemotingListener(
            Action<TRemotingReplicaTemplate> declareAction);
    }
}