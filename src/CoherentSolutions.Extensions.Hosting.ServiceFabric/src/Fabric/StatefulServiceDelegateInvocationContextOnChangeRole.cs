using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnChangeRole 
        : StatefulServiceDelegateInvocationContext<IStatefulServiceEventPayloadOnChangeRole>,
        IStatefulServiceDelegateInvocationContextOnChangeRole
    {
        public StatefulServiceDelegateInvocationContextOnChangeRole(
            IStatefulServiceEventPayloadOnChangeRole payload)
            : base(StatefulServiceLifecycleEvent.OnChangeRole, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}