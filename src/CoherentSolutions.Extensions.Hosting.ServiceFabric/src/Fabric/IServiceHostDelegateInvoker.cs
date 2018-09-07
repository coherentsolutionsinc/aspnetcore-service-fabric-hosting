using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateInvoker<in TInvocationContext>
    {
        Task InvokeAsync(
            TInvocationContext context,
            CancellationToken cancellationToken);
    }
}