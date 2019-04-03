namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContextOnDataLoss
        : IStatefulServiceDelegateInvocationContext
    {
        IStatefulServiceEventPayloadOnDataLoss Payload { get; }
    }
}