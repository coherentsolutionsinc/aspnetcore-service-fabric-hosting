using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseStatefulDelegateEventTheoryExtension : IUseDelegateEventTheoryExtension<StatefulServiceLifecycleEvent>
    {
        public StatefulServiceLifecycleEvent Event { get; private set; }

        public UseStatefulDelegateEventTheoryExtension()
        {
            this.Event = StatefulServiceLifecycleEvent.OnRunAfterListenersOpened;
        }

        public UseStatefulDelegateEventTheoryExtension Setup(
            StatefulServiceLifecycleEvent @event)
        {
            this.Event = @event;

            return this;
        }
    }
}