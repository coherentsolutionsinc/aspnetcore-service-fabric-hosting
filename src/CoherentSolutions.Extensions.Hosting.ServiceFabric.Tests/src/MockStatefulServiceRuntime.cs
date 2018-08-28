using System;
using System.Fabric;

using Microsoft.ServiceFabric.Services.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class MockStatefulServiceRuntime
    {
        private readonly Func<StatefulServiceContext, StatefulServiceBase> serviceFactory;

        public MockStatefulServiceRuntime(
            Func<StatefulServiceContext, StatefulServiceBase> serviceFactory)
        {
            this.serviceFactory = serviceFactory 
             ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public MockStatefulServiceReplica CreateInstance(
            StatefulServiceContext serviceContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }

            var service = this.serviceFactory(serviceContext);

            return new MockStatefulServiceReplica(service, serviceContext);
        }
    }
}