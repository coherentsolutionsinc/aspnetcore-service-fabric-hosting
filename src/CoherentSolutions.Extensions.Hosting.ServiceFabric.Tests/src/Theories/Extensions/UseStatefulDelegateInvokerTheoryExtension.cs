using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseStatefulDelegateInvokerTheoryExtension 
        : IUseDelegateInvokerTheoryExtension<IStatefulServiceDelegateInvocationContext>
    {
        public Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker<IStatefulServiceDelegateInvocationContext>> Factory { get; private set; }

        public UseStatefulDelegateInvokerTheoryExtension()
        {
            this.Factory = (
                @delegate,
                provider) => new StatefulServiceHostDelegateInvoker(@delegate, provider);
        }

        public UseStatefulDelegateInvokerTheoryExtension Setup(
            Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker<IStatefulServiceDelegateInvocationContext>> factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}