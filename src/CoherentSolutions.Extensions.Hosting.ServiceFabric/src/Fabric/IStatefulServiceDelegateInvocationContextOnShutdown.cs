namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContextOnShutdown
        : IStatefulServiceDelegateInvocationContext
    {
        IStatefulServiceEventPayloadOnShutdown Payload { get; }
    }
}