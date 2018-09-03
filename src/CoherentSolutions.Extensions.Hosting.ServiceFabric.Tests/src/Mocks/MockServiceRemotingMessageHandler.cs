using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

using ServiceFabric.Mocks.RemotingV2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockServiceRemotingMessageHandler : IServiceRemotingMessageHandler
    {
        public Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage)
        {
            return Task.FromResult((IServiceRemotingResponseMessage) new MockServiceRemotingResponseMessage());
        }

        public void HandleOneWayMessage(
            IServiceRemotingRequestMessage requestMessage)
        {
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return new MockServiceRemotingMessageBodyFactory();
        }
    }
}