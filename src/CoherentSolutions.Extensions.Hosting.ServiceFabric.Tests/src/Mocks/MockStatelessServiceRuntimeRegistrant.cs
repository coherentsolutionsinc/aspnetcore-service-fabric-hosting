using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockStatelessServiceRuntimeRegistrant : IStatelessServiceRuntimeRegistrant
    {
        private MockStatelessServiceInstance serviceInstance;

        public Task RegisterAsync(
            string serviceTypeName,
            Func<StatelessServiceContext, StatelessService> serviceFactory,
            CancellationToken cancellationToken)
        {
            var context = MockStatelessServiceContextFactory.Create(
                MockCodePackageActivationContext.Default,
                serviceTypeName,
                new Uri(MockStatelessServiceContextFactory.ServiceName),
                Guid.Empty,
                default);

            this.serviceInstance = new MockStatelessServiceInstance(serviceFactory(context));
            return this.serviceInstance.InitiateStartupSequenceAsync();
        }

        public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            return this.serviceInstance.InitiateShutdownSequenceAsync();
        }
    }
}