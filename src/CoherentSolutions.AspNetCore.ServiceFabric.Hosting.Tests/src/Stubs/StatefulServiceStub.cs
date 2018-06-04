using System.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using Microsoft.ServiceFabric.Data;
using Moq;

using ServiceFabric.Mocks;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs
{
    internal class StatefulServiceStub : IStatefulService
    {
        public IReliableStateManager GetReliableStateManager()
        {
            return new Mock<IReliableStateManager>().Object;
        }

        public ServiceContext GetContext()
        {
            return MockStatefulServiceContextFactory.Default;
        }

        public IServiceEventSource GetEventSource()
        {
            return new Mock<IServiceEventSource>().Object;
        }

        public IServicePartition GetPartition()
        {
            return new Mock<IStatefulServicePartition>().Object;
        }
    }
}