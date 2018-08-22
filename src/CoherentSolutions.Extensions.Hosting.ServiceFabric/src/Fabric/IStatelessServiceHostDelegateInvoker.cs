using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostDelegateInvoker
    {
        Task InvokeAsync(
            IStatelessServiceDelegateInvocationContext context,
            CancellationToken cancellationToken);
    }
}