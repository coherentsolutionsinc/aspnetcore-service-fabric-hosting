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
        private MockStatefulServiceReplica serviceInstance;

        public Task RegisterAsync(
            string serviceTypeName,
            Func<StatefulServiceContext, StatefulServiceBase> serviceFactory,
            CancellationToken cancellationToken)
        {
            var serviceRuntime = MockServiceRuntimeFactory.CreateStatefulServiceRuntime(serviceFactory);

            this.serviceInstance = serviceRuntime.CreateInstance(
                MockStatefulServiceContextFactory.Create(
                    MockServiceCodePackageActivationContext.Default,
                    serviceTypeName,
                    new Uri(MockStatefulServiceContextFactory.ServiceName),
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