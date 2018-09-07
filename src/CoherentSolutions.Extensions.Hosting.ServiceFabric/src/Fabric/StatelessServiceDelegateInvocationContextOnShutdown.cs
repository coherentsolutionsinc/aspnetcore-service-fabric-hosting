using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextOnShutdown
        : StatelessServiceDelegateInvocationContext<IStatelessServiceEventPayloadOnShutdown>,
          IStatelessServiceDelegateInvocationContextOnShutdown
    {
        public StatelessServiceDelegateInvocationContextOnShutdown(
            IStatelessServiceEventPayloadOnShutdown payload)
            : base(StatelessServiceLifecycleEvent.OnShutdown, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}