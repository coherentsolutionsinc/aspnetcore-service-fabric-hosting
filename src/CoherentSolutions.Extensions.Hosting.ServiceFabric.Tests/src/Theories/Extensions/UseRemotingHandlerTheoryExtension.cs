using System;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

using ServiceFabric.Mocks.RemotingV2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingHandlerTheoryExtension : IUseRemotingHandlerTheoryExtension
    {
        private sealed class RemotingHandler : IServiceRemotingMessageHandler
        {
            public Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
                IServiceRemotingRequestContext requestContext,
                IServiceRemotingRequestMessage requestMessage)
            {
                return Task.FromResult((IServiceRemotingResponseMessage)new MockServiceRemotingResponseMessage());
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

        public UseRemotingHandlerTheoryExtension()
        {
            this.Factory = Tools.GetRemotingHandlerFunc<RemotingHandler>();
        }

        public UseRemotingHandlerTheoryExtension Setup<T>()
            where T : IServiceRemotingMessageHandler
        {
            this.Factory = Tools.GetRemotingHandlerFunc<T>();

            return this;
        }

        public UseRemotingHandlerTheoryExtension Setup(
            Func<IServiceProvider, IServiceRemotingMessageHandler> factory)
        {
            this.Factory = factory
             ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}