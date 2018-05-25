using System.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using Microsoft.ServiceFabric.Data;
using Moq;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs
{
    internal class StatefulServiceStub : IStatefulService
    {
        public IReliableStateManager GetReliableStateManager()
        {
            return new Mock<IReliableStateManager>().Object;
        }

        public IServicePartition GetPartition()
        {
            return new Mock<IStatefulServicePartition>().Object;
        }
    }
}