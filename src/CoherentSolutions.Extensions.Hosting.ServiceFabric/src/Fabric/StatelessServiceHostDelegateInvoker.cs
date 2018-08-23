using System;

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
    }
}