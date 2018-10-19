namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceReplicableTemplate<in TServiceInformation, out TEventSource>
        where TServiceInformation : IServiceInformation
    {
        TEventSource Activate(
            TServiceInformation serviceContext);
    }
}