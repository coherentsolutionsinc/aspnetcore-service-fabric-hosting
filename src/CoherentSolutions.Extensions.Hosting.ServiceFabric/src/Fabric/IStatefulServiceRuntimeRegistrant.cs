using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceRuntimeRegistrant
    {
        Task RegisterAsync(
            string serviceTypeName,
            Func<StatefulServiceContext, StatefulService> serviceFactory,
            CancellationToken cancellationToken);

        Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken);
    }
}