namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContextOnPackageModified<out TPackage>
        : IStatefulServiceDelegateInvocationContext
    {
        IServiceEventPayloadOnPackageModified<TPackage> Payload { get; }
    }
}