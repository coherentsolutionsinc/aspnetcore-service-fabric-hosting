using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public class MockFabricTransportServiceRemotingListener : FabricTransportServiceRemotingListener, ICommunicationListener
    {
        public MockFabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IService serviceImplementation,
            FabricTransportRemotingListenerSettings remotingListenerSettings = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null)
            : base(serviceContext, serviceImplementation, remotingListenerSettings, serializationProvider)
        {
        }

        public MockFabricTransportServiceRemotingListener(
            ServiceContext serviceContext,
            IServiceRemotingMessageHandler serviceRemotingMessageHandler,
            FabricTransportRemotingListenerSettings remotingListenerSettings = null,
            IServiceRemotingMessageSerializationProvider serializationProvider = null)
            : base(serviceContext, serviceRemotingMessageHandler, remotingListenerSettings, serializationProvider)
        {
        }

        void ICommunicationListener.Abort()
        {
        }

        Task<string> ICommunicationListener.OpenAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult(String.Empty);
        }

        Task ICommunicationListener.CloseAsync(
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}