namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceReplicator<in TServiceInformation, out TEventSource>
    {
        TEventSource ReplicateFor(
            TServiceInformation serviceContext);
    }
}