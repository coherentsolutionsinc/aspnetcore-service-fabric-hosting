using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostRuntime
    {
        Task RegisterServiceAsync(
            string serviceTypeName,
            Func<StatefulServiceContext, StatefulService> serviceFactory,
            TimeSpan timeout = default,
            CancellationToken cancellationToken = default);

        Task RegisterServiceAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            TimeSpan timeout = default,
            CancellationToken cancellationToken = default);
    }
}