using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;

using ServiceFabric.Mocks.RemotingV2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks
{
    public class MockServiceRemotingRequestMessageBodySerializer : IServiceRemotingRequestMessageBodySerializer
    {
        public IOutgoingMessageBody Serialize(
            IServiceRemotingRequestMessageBody serviceRemotingRequestMessageBody)
        {
            return null;
        }

        public IServiceRemotingRequestMessageBody Deserialize(
            IIncomingMessageBody messageBody)
        {
            return new MockServiceRemotingRequestMessageBody();
        }
    }
}