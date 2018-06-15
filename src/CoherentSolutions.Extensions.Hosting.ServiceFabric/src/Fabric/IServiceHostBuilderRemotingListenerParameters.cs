using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderRemotingListenerParameters<out TReplicaTemplate>
        where TReplicaTemplate : IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>
    {
        Func<TReplicaTemplate> RemotingListenerReplicaTemplateFunc { get; }
    }
}