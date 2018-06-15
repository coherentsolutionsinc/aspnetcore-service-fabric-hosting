namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostRemotingListenerReplicaTemplate<out TConfigurator>
        : IServiceHostListenerReplicaTemplate<TConfigurator>
        where TConfigurator : IServiceHostRemotingListenerReplicaTemplateConfigurator
    {
    }
}