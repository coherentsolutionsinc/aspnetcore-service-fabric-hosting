using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostDelegateInvoker
    {
        Task InvokeAsync(
            IStatefulServiceDelegateInvocationContext context,
            CancellationToken cancellationToken);
    }
}