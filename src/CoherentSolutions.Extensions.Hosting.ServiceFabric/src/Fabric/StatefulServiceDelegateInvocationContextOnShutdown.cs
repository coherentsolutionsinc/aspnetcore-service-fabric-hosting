using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnShutdown 
        : StatefulServiceDelegateInvocationContext<IStatefulServiceEventPayloadOnShutdown>,
          IStatefulServiceDelegateInvocationContextOnShutdown
    {
        public StatefulServiceDelegateInvocationContextOnShutdown(
            IStatefulServiceEventPayloadOnShutdown payload)
            : base(StatefulServiceLifecycleEvent.OnShutdown, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}