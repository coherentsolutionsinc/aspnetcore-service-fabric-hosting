using System;

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
    }
}