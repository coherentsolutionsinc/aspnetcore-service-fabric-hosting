using System;
using System.Linq;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseDelegateInvokerTheoryExtension
        : IUseDelegateInvokerTheoryExtension
    {
        public Func<IServiceProvider, IServiceDelegateInvoker> Factory { get; private set; }

        public UseDelegateInvokerTheoryExtension()
        {
            this.Factory = provider => new ServiceDelegateInvoker(
                Enumerable.Empty<IServiceDelegateInvocationContextRegistrant>(),
                provider);
        }

        public UseDelegateInvokerTheoryExtension Setup(
            Func<IServiceProvider, IServiceDelegateInvoker> factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}