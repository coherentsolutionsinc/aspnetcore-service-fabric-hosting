using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderRemotingListenerParameters<out TReplicaTemplate>
        where TReplicaTemplate : IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>
    {
        Func<TReplicaTemplate> RemotingListenerReplicaTemplateFunc { get; }
    }
}