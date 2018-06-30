using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceRuntimeRegistrant
    {
        Task RegisterAsync(
            string serviceTypeName,
            Func<StatefulServiceContext, StatefulServiceBase> serviceFactory,
            CancellationToken cancellationToken);
    }
}