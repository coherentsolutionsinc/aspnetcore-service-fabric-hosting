using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnDataLoss
        : StatefulServiceDelegateInvocationContext<IStatefulServiceEventPayloadOnDataLoss>,
          IStatefulServiceDelegateInvocationContextOnDataLoss
    {
        public StatefulServiceDelegateInvocationContextOnDataLoss(
            IStatefulServiceEventPayloadOnDataLoss payload)
            : base(StatefulServiceLifecycleEvent.OnDataLoss, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}