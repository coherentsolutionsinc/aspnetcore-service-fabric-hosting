namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostEventSourceReplicableTemplate
        : IServiceHostEventSourceReplicableTemplate<IStatelessServiceInformation, StatelessServiceEventSource>
    {
    }
}