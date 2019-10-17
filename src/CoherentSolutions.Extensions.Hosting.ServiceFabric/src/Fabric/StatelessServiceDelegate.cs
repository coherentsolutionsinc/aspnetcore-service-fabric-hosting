using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegate
    {
        public StatelessServiceLifecycleEvent Event
        {
            get;
        }

        public Delegate Delegate
        {
            get; private set;
        }

        public Func<IServiceDelegateInvoker> CreateDelegateInvoker
        {
            get;
        }

        public StatelessServiceDelegate(
            StatelessServiceLifecycleEvent @event,
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