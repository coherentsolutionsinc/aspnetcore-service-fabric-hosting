using System;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

using ServiceFabric.Mocks.RemotingV2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingListenerHandlerTheoryExtension : IUseRemotingListenerHandlerTheoryExtension
    {
        private sealed class Handler : IServiceRemotingMessageHandler
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

        public Func<IServiceProvider, IServiceRemotingMessageHandler> Factory { get; private set; }

        public UseRemotingListenerHandlerTheoryExtension()
        {
            this.Factory = Tools.GetRemotingHandlerFunc<Handler>();
        }

        public UseRemotingListenerHandlerTheoryExtension Setup<T>()
            where T : IServiceRemotingMessageHandler
        {
            this.Factory = Tools.GetRemotingHandlerFunc<T>();

            return this;
        }

        public UseRemotingListenerHandlerTheoryExtension Setup(
            Func<IServiceProvider, IServiceRemotingMessageHandler> factory)
        {
            this.Factory = factory
             ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}