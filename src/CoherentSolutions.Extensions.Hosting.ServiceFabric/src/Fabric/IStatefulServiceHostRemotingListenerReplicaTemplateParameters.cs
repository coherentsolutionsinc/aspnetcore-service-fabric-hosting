namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostRemotingListenerReplicaTemplateParameters
        : IStatefulServiceHostListenerReplicaTemplateParameters,
          IServiceHostRemotingListenerReplicaTemplateParameters
    {
    }
}