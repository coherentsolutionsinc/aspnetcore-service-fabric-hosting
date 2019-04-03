namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContextOnChangeRole
        : IStatefulServiceDelegateInvocationContext
    {
        IStatefulServiceEventPayloadOnChangeRole Payload { get; }
    }
}