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

    public class StatelessServiceDelegateInvocationContext<TPayload> : StatelessServiceDelegateInvocationContext
    {
        public TPayload Payload { get; }

        public StatelessServiceDelegateInvocationContext(
            StatelessServiceLifecycleEvent @event,
            TPayload payload)
            : base(@event)
        {
            this.Payload = payload;
        }
    }
}