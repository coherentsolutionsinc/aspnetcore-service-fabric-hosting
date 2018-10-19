using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseStatelessDelegateInvokerTheoryExtension
        : IUseDelegateInvokerTheoryExtension<IStatelessServiceDelegateInvocationContext>
    {
        public Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker<IStatelessServiceDelegateInvocationContext>> Factory { get; private set; }

        public UseStatelessDelegateInvokerTheoryExtension()
        {
            this.Factory = (
                @delegate,
                provider) => new StatelessServiceHostDelegateInvoker(@delegate, provider);
        }

        public UseStatelessDelegateInvokerTheoryExtension Setup(
            Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker<IStatelessServiceDelegateInvocationContext>> factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}