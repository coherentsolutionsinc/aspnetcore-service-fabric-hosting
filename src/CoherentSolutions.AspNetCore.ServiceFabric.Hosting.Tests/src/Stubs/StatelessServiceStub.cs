using System.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using Moq;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs
{
    internal class StatelessServiceStub : IStatelessService
    {
        public IServicePartition GetPartition()
        {
            return new Mock<IStatelessServicePartition>().Object;
        }
    }
}