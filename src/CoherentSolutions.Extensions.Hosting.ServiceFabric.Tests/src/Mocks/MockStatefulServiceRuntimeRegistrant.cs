﻿using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockStatefulServiceRuntimeRegistrant : IStatefulServiceRuntimeRegistrant
    {
        private MockStatefulServiceReplica serviceReplica;

        public Task RegisterAsync(
            string serviceTypeName,
            Func<StatefulServiceContext, StatefulService> serviceFactory,
            CancellationToken cancellationToken)
        {
            var context = MockStatefulServiceContextFactory.Create(
                MockCodePackageActivationContext.Default,
                serviceTypeName,
                new Uri(MockStatefulServiceContextFactory.ServiceName),
                Guid.Empty,
                default);

            this.serviceReplica = new MockStatefulServiceReplica(serviceFactory(context));
            return this.serviceReplica.InitiateStartupSequenceAsync();
        }

        public Task UnregisterAsync(
            string serviceTypeName,
            CancellationToken cancellationToken)
        {
            return this.serviceReplica.InitiateShutdownSequenceAsync();
        }
    }
}