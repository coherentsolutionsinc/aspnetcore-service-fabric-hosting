namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContext
        : IStatefulServiceDelegateInvocationContext
    {
        public StatefulServiceLifecycleEvent Event { get; }

        public StatefulServiceDelegateInvocationContext(
            StatefulServiceLifecycleEvent @event)
        {
            this.Event = @event;
        }
    }
}