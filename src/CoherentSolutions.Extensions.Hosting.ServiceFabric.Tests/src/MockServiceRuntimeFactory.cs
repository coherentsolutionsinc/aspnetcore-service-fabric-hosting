using System;
using System.Fabric;

using Microsoft.ServiceFabric.Services.Runtime;

using StatelessService = CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.StatelessService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public static class MockServiceRuntimeFactory
    {
        public static MockStatefulServiceRuntime CreateStatefulServiceRuntime(
            Func<StatefulServiceContext, StatefulServiceBase> serviceFactory)
        {
            if (serviceFactory == null)
            {
                throw new ArgumentNullException(nameof(serviceFactory));
            }

            return new MockStatefulServiceRuntime(serviceFactory);
        }

        public static MockStatelessServiceRuntime CreateStatelessServiceRuntime(
            Func<StatelessServiceContext, StatelessService> serviceFactory)
        {
            if (serviceFactory == null)
            {
                throw new ArgumentNullException(nameof(serviceFactory));
            }

            return new MockStatelessServiceRuntime(serviceFactory);
        }
    }
}