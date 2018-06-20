using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateInvoker
    {
        Task InvokeAsync(
            IEnumerable<IServiceHostDelegate> delegates,
            CancellationToken cancellationToken);
    }
}