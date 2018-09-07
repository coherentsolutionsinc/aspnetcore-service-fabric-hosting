using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostDelegateInvoker
        : ServiceHostDelegateInvoker<IStatelessServiceDelegateInvocationContext>,
          IStatelessServiceHostDelegateInvoker
    {
        public StatelessServiceHostDelegateInvoker(
            Delegate @delegate,
            IServiceProvider services)
            : base(@delegate, services)
        {
        }

        protected override IEnumerable<(Type t, object o)> UnwrapInvocationContext(
            IStatelessServiceDelegateInvocationContext invocationContext)
        {
            switch (invocationContext)
            {
                case StatelessServiceDelegateInvocationContext<IStatelessServiceEventPayloadOnShutdown> ctx:
                    {
                        yield return (typeof(IStatelessServiceEventPayloadOnShutdown), ctx.Payload);
                    }
                    break;
            }
        }
    }
}