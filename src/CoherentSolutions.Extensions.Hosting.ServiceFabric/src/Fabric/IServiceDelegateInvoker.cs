using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceDelegateInvoker
    {
        Task InvokeAsync(
            Delegate @delegate,
            IServiceDelegateInvocationContext invocationContext,
            CancellationToken cancellationToken);
    }
}