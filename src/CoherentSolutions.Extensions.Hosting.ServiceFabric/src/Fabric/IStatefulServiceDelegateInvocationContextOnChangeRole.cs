namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContextOnChangeRole
    {
        IStatefulServiceEventPayloadOnChangeRole Payload { get; }
    }
}