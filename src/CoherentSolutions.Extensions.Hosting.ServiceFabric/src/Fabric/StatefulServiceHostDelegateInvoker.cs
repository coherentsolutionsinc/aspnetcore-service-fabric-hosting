using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostDelegateInvoker
        : ServiceHostDelegateInvoker<IStatefulServiceDelegateInvocationContext>,
          IStatefulServiceHostDelegateInvoker
    {
        public StatefulServiceHostDelegateInvoker(
            Delegate @delegate,
            IServiceProvider services)
            : base(@delegate, services)
        {
        }

        protected override IEnumerable<(Type t, object o)> UnwrapInvocationContext(
            IStatefulServiceDelegateInvocationContext invocationContext)
        {
            switch (invocationContext)
            {
                case StatefulServiceDelegateInvocationContext<IStatefulServiceEventPayloadOnChangeRole> ctx:
                    {
                        yield return (typeof(IStatefulServiceEventPayloadOnChangeRole), ctx.Payload);
                    }
                    break;
                case StatefulServiceDelegateInvocationContext<IStatefulServiceEventPayloadOnShutdown> ctx:
                    {
                        yield return (typeof(IStatefulServiceEventPayloadOnShutdown), ctx.Payload);
                    }
                    break;
                case StatefulServiceDelegateInvocationContext<IStatefulServiceEventPayloadOnDataLoss> ctx:
                    {
                        yield return (typeof(IStatefulServiceEventPayloadOnShutdown), ctx.Payload);
                    }
                    break;
            }
        }
    }
}