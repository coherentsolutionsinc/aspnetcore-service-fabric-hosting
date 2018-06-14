namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostRemotingListenerReplicaTemplate<out TConfigurator>
        : IServiceHostListenerReplicaTemplate<TConfigurator>
        where TConfigurator : IServiceHostRemotingListenerReplicaTemplateConfigurator
    {
    }
}