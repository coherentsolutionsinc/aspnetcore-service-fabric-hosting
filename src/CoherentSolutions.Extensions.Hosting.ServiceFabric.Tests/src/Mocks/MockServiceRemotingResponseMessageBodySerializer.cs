using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

using ServiceFabric.Mocks.RemotingV2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockServiceRemotingResponseMessageBodySerializer : IServiceRemotingResponseMessageBodySerializer
    {
        public IOutgoingMessageBody Serialize(
            IServiceRemotingResponseMessageBody serviceRemotingResponseMessageBody)
        {
            return null;
        }

        public IServiceRemotingResponseMessageBody Deserialize(
            IIncomingMessageBody messageBody)
        {
            return new MockServiceRemotingResponseMessageBody();
        }
    }
}