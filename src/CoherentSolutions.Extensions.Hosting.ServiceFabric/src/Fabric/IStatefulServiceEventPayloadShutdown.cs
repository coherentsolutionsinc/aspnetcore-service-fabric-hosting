namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceEventPayloadShutdown
    {
        bool IsAborting { get; }
    }
}