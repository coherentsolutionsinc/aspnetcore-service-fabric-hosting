using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegate
    {
        public StatefulServiceLifecycleEvent Event { get; }

        public Func<IStatefulServiceHostDelegateInvoker> CreateDelegateInvokerFunc { get; }

        public StatefulServiceDelegate(
            Func<IStatefulServiceHostDelegateInvoker> delegateInvokerFunc,
            StatefulServiceLifecycleEvent @event)
        {
            this.Event = @event;
            this.CreateDelegateInvokerFunc = delegateInvokerFunc
             ?? throw new ArgumentNullException(nameof(delegateInvokerFunc));
        }
    }
}