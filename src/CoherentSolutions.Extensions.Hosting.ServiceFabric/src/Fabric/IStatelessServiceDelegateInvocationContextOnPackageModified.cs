namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceDelegateInvocationContextOnPackageModified<out TPackage>
        : IStatelessServiceDelegateInvocationContext
    {
        IServiceEventPayloadOnPackageModified<TPackage> Payload { get; }
    }
}