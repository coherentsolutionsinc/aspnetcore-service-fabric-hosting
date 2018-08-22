using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegate
    {
        public StatelessServiceLifecycleEvent Event { get; }

        public Func<IStatelessServiceHostDelegateInvoker> CreateDelegateInvokerFunc { get; }

        public StatelessServiceDelegate(
            Func<IStatelessServiceHostDelegateInvoker> delegateInvokerFunc,
            StatelessServiceLifecycleEvent @event)
        {
            this.Event = @event;
            this.CreateDelegateInvokerFunc = delegateInvokerFunc
             ?? throw new ArgumentNullException(nameof(delegateInvokerFunc));
        }
    }
}