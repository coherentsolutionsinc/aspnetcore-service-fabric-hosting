using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockStatelessService : IStatelessService
    {
        private readonly StatelessServiceContext serviceContext;

        private readonly IServiceEventSource serviceEventSource;

        private readonly IServicePartition servicePartition;

        public MockStatelessService()
            : this(MockStatelessServiceContextFactory.Default, new MockServiceEventSource(), new MockStatelessServicePartition())
        {
        }

        public MockStatelessService(
            StatelessServiceContext serviceContext,
            IServiceEventSource serviceEventSource,
            IServicePartition servicePartition)
        {
            this.serviceContext = serviceContext
             ?? throw new ArgumentNullException(nameof(serviceContext));

            this.serviceEventSource = serviceEventSource
             ?? throw new ArgumentNullException(nameof(serviceEventSource));

            this.servicePartition = servicePartition
             ?? throw new ArgumentNullException(nameof(servicePartition));
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
    }
}