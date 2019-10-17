namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceDelegateInvocationContext : IServiceDelegateInvocationContext
    {
        StatelessServiceLifecycleEvent Event { get; }
    }
}