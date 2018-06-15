using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
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