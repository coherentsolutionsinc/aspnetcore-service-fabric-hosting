namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceEventPayloadShutdown
    {
        bool IsAborting { get; }
    }
}