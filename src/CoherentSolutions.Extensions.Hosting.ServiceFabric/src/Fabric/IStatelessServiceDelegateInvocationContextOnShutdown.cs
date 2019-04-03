namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceDelegateInvocationContextOnShutdown
        : IStatelessServiceDelegateInvocationContext
    {
        IStatelessServiceEventPayloadOnShutdown Payload { get; }
    }
}