using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class TestRemotingHandlerWithParameters : IServiceRemotingMessageHandler

    {
        public ITestDependency Dependency { get; }

        public TestRemotingHandlerWithParameters(
            ITestDependency dependency)
        {
            this.Dependency = dependency;
        }

        public Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage)
        {
            return null;
        }

        public void HandleOneWayMessage(
            IServiceRemotingRequestMessage requestMessage)
        {
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return null;
        }
    }
}