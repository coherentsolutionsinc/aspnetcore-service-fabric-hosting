using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegate
    {
        public StatefulServiceLifecycleEvent Event
        {
            get;
        }

        public Delegate Delegate
        {
            get;
        }

        public Func<IServiceDelegateInvoker> CreateDelegateInvoker
        {
            get;
        }

        public StatefulServiceDelegate(
            StatefulServiceLifecycleEvent @event,
            Delegate @delegate,
            Func<IServiceDelegateInvoker> factory)
        {
            this.Event = @event;

            this.Delegate = @delegate
                ?? throw new ArgumentNullException(nameof(@delegate));

            this.CreateDelegateInvoker = factory
                ?? throw new ArgumentNullException(nameof(factory));
        }
    }
}