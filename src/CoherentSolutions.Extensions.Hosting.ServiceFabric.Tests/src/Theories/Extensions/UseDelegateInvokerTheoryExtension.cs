using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseDelegateInvokerTheoryExtension : IUseDelegateInvokerTheoryExtension
    {
        public Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker> Factory { get; private set; }

        public UseDelegateInvokerTheoryExtension()
        {
            this.Factory = HostingDefaults.DefaulDelegateInvokerFunc;
        }

        public UseDelegateInvokerTheoryExtension Setup(
            Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker> factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}