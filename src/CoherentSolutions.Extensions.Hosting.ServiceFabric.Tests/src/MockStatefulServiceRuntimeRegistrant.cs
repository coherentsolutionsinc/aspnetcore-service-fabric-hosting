using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.ServiceFabric.Services.Runtime;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class MockStatefulServiceRuntimeRegistrant : IStatefulServiceRuntimeRegistrant
    {
        private MockStatefulServiceReplica serviceReplica;

        public Task RegisterAsync(
            string serviceTypeName,
            Func<StatefulServiceContext, StatefulServiceBase> serviceFactory,
            CancellationToken cancellationToken)
        {
            this.serviceReplica = new MockStatefulServiceReplica(
                serviceFactory,
                MockStatefulServiceContextFactory.Create(
                    MockCodePackageActivationContext.Default,
                    serviceTypeName,
                    new Uri(MockStatefulServiceContextFactory.ServiceName),
                    Guid.Empty,
                    default));

            return this.serviceReplica.StartAsync();
        }

        public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            return this.serviceReplica.StopAsync();
        }
    }
}