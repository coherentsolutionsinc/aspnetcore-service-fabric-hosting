namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceDelegateInvocationContextOnShutdown
    {
        IStatelessServiceEventPayloadOnShutdown Payload { get; }
    }
}