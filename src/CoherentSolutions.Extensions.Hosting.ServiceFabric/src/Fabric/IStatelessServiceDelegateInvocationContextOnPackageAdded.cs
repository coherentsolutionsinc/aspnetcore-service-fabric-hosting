namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceDelegateInvocationContextOnPackageAdded<out TPackage>
        : IStatelessServiceDelegateInvocationContext
    {
        IServiceEventPayloadOnPackageAdded<TPackage> Payload { get; }
    }
}