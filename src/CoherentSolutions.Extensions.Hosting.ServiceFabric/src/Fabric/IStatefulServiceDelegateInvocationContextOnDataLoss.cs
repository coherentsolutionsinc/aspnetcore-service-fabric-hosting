namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContextOnDataLoss
    {
        IStatefulServiceEventPayloadOnDataLoss Payload { get; }
    }
}