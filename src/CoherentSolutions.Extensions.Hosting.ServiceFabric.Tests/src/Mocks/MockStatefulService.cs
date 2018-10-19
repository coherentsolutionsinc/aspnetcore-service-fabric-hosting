using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.ServiceFabric.Data;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockStatefulService : IStatefulService
    {
        private readonly StatefulServiceContext serviceContext;

        private readonly IServiceEventSource serviceEventSource;

        private readonly IServicePartition servicePartition;

        private readonly IReliableStateManager reliableStateManager;

        public MockStatefulService()
            : this(MockStatefulServiceContextFactory.Default, new MockServiceEventSource(), new MockStatefulServicePartition(), new MockReliableStateManager())
        {
        }

        public MockStatefulService(
            StatefulServiceContext serviceContext,
            IServiceEventSource serviceEventSource,
            IServicePartition servicePartition,
            IReliableStateManager reliableStateManager)
        {
            this.serviceContext = serviceContext
             ?? throw new ArgumentNullException(nameof(serviceContext));

            this.serviceEventSource = serviceEventSource
             ?? throw new ArgumentNullException(nameof(serviceEventSource));

            this.servicePartition = servicePartition
             ?? throw new ArgumentNullException(nameof(servicePartition));

            this.reliableStateManager = reliableStateManager;
        }

        public ServiceContext GetContext()
        {
            return this.serviceContext;
        }

        public IServiceEventSource GetEventSource()
        {
            return this.serviceEventSource;
        }

        public IServicePartition GetPartition()
        {
            return this.servicePartition;
        }

        public IReliableStateManager GetReliableStateManager()
        {
            return this.reliableStateManager;
        }
    }
}