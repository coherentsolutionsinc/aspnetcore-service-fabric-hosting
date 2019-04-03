namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContextOnPackageRemoved<out TPackage>
        : IStatefulServiceDelegateInvocationContext
    {
        IServiceEventPayloadOnPackageRemoved<TPackage> Payload { get; }
    }
}