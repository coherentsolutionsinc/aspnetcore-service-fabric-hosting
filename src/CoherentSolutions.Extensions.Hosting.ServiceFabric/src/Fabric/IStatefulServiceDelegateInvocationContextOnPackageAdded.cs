namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContextOnPackageAdded<out TPackage>
        : IStatefulServiceDelegateInvocationContext
    {
        IServiceEventPayloadOnPackageAdded<TPackage> Payload { get; }
    }
}