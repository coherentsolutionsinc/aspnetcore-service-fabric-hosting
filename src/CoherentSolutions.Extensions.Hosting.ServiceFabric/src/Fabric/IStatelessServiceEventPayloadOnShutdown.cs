namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceEventPayloadOnShutdown
    {
        bool IsAborting { get; }
    }
}