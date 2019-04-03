namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceDelegateInvocationContextOnPackageRemoved<out TPackage>
        : IStatelessServiceDelegateInvocationContext
    {
        IServiceEventPayloadOnPackageRemoved<TPackage> Payload { get; }
    }
}