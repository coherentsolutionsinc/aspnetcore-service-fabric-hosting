using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using ServiceFabric.Mocks;

using StatelessService = CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.StatelessService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class MockStatelessServiceRuntimeRegistrant : IStatelessServiceRuntimeRegistrant
    {
        private MockStatelessServiceInstance serviceInstance;

        public Task RegisterAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            CancellationToken cancellationToken)
        {
            this.serviceInstance = new MockStatelessServiceInstance(
                serviceFactory,
                MockStatelessServiceContextFactory.Create(
                    MockCodePackageActivationContext.Default,
                    serviceTypeName,
                    new Uri(MockStatelessServiceContextFactory.ServiceName),
                    Guid.Empty,
                    default));

            return this.serviceInstance.CreateAsync();
        }

        public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            return this.serviceInstance.DestroyAsync();
        }
    }
}