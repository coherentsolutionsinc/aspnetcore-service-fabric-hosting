using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostDelegateInvoker
        : IServiceHostDelegateInvoker
    {
        public async Task InvokeAsync(
            IEnumerable<IServiceHostDelegate> delegates,
            CancellationToken cancellationToken)
        {
            foreach (var @delegate in delegates)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await @delegate.InvokeAsync(cancellationToken);
            }
        }
    }
}