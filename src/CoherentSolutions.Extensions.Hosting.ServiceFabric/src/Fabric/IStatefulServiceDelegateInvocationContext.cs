namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContext : IServiceDelegateInvocationContext
    {
        StatefulServiceLifecycleEvent Event { get; }
    }
}