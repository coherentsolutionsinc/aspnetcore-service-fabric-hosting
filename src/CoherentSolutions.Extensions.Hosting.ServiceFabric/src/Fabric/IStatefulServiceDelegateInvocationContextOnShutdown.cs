namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContextOnShutdown
    {
        IStatefulServiceEventPayloadOnShutdown Payload { get; }
    }
}