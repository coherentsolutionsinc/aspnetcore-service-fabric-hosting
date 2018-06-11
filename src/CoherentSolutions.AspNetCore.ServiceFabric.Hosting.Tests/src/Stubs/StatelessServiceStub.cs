using System.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using Moq;

using ServiceFabric.Mocks;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs
{
    internal class StatelessServiceStub : IStatelessService
    {
        public ServiceContext GetContext()
        {
            return MockStatelessServiceContextFactory.Default;
        }

        public IServiceEventSource GetEventSource()
        {
            return new Mock<IServiceEventSource>().Object;
        }

        public IServicePartition GetPartition()
        {
            return new Mock<IStatelessServicePartition>().Object;
        }
    }
}