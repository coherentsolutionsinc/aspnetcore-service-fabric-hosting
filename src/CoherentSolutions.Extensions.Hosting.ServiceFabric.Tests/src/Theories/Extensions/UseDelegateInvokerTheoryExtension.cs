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

    public class UseDelegateInvokerTheoryExtension : IUseDelegateInvokerTheoryExtension<object>
    {
        public Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker<object>> Factory { get; private set; }

        public UseDelegateInvokerTheoryExtension()
        {
            this.Factory = Tools.GetDelegateInvokerFunc();
        }

        public UseDelegateInvokerTheoryExtension Setup(
            Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker<object>> factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}