namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceEventPayloadOnShutdown
    {
        bool IsAborting { get; }
    }
}