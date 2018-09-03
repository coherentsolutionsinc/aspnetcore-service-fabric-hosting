using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseStatelessDelegateEventTheoryExtension : IUseDelegateEventTheoryExtension<StatelessServiceLifecycleEvent>
    {
        public StatelessServiceLifecycleEvent Event { get; private set; }

        public UseStatelessDelegateEventTheoryExtension()
        {
            this.Event = StatelessServiceLifecycleEvent.OnRun;
        }

        public UseStatelessDelegateEventTheoryExtension Setup(
            StatelessServiceLifecycleEvent @event)
        {
            this.Event = @event;

            return this;
        }
    }
}