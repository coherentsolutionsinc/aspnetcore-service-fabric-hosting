namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContext
        : IStatelessServiceDelegateInvocationContext
    {
        public StatelessServiceLifecycleEvent Event { get; }

        public StatelessServiceDelegateInvocationContext(
            StatelessServiceLifecycleEvent @event)
        {
            this.Event = @event;
        }
    }
}