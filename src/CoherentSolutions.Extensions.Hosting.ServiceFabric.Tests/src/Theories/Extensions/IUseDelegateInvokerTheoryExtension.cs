using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseDelegateInvokerTheoryExtension<in TInvocationContext>
    {
        Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker<TInvocationContext>> Factory { get; }
    }
}