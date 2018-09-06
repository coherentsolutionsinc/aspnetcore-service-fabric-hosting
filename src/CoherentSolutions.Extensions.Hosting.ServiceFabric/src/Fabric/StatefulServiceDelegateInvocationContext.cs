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

    public class StatefulServiceDelegateInvocationContext<TPayload>
        : StatefulServiceDelegateInvocationContext
    {
        public TPayload Payload { get; }

        public StatefulServiceDelegateInvocationContext(
            StatefulServiceLifecycleEvent @event,
            TPayload payload)
            : base(@event)
        {
            this.Payload = payload;
        }
    }
}