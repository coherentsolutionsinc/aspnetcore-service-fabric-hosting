namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostEventSourceReplicator
        : IServiceHostEventSourceReplicator<IStatefulServiceInformation, StatefulServiceEventSource>
    {
    }
}