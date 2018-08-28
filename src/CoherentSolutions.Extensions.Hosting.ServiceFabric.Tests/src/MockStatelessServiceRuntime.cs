using System;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;

using ServiceFabric.Mocks;

using StatelessService = Microsoft.ServiceFabric.Services.Runtime.StatelessService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class MockStatelessServiceRuntime
    {
        private readonly Func<StatelessServiceContext, StatelessService> serviceFactory;

        public MockStatelessServiceRuntime(
            Func<StatelessServiceContext, StatelessService> serviceFactory)
        {
            this.serviceFactory = serviceFactory 
             ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public MockStatelessServiceInstance CreateInstance(
            StatelessServiceContext serviceContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }

            var service = this.serviceFactory(serviceContext);

            return new MockStatelessServiceInstance(service, serviceContext);
        }
    }
}