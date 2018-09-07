namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceDelegateInvocationContext
    {
        StatelessServiceLifecycleEvent Event { get; }
    }
}