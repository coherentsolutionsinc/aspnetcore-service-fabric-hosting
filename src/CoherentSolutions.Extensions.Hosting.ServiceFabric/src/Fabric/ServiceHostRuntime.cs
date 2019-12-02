using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostRuntime : IServiceHostRuntime
    {
        public Task RegisterServiceAsync(
            string serviceTypeName,
            Func<StatefulServiceContext, StatefulService> serviceFactory,
            TimeSpan timeout = default,
            CancellationToken cancellationToken = default)
        {
            return ServiceRuntime.RegisterServiceAsync(serviceTypeName, serviceFactory, cancellationToken: cancellationToken);
        }

        public Task RegisterServiceAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            TimeSpan timeout = default,
            CancellationToken cancellationToken = default)
        {
            return ServiceRuntime.RegisterServiceAsync(serviceTypeName, serviceFactory, cancellationToken: cancellationToken);
        }
    }
}