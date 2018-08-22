namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceDelegateInvocationContext
    {
        StatefulServiceLifecycleEvent Event { get; }
    }
}