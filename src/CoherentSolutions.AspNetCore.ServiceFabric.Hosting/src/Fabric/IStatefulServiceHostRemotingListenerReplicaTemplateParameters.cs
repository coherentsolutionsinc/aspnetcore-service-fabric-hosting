namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostRemotingListenerReplicaTemplateParameters
        : IStatefulServiceListenerReplicaTemplateParameters,
          IServiceHostRemotingListenerReplicaTemplateParameters
    {
    }
}