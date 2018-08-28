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
            var serviceRuntime = MockServiceRuntimeFactory.CreateStatelessServiceRuntime(serviceFactory);

            this.serviceInstance = serviceRuntime.CreateInstance(
                MockStatelessServiceContextFactory.Create(
                    MockServiceCodePackageActivationContext.Default,
                    serviceTypeName,
                    new Uri(MockStatelessServiceContextFactory.ServiceName),
                    Guid.Empty,
                    default));

            return this.serviceInstance.StartAsync();
        }

        public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            return this.serviceInstance.StopAsync();
        }
    }
}