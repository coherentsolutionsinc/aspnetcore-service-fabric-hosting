using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateInvoker
    {
        Task InvokeAsync(
            CancellationToken cancellationToken);
    }
}