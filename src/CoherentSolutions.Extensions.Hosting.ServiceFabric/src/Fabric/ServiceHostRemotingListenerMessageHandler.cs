using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostRemotingListenerMessageHandler : IServiceRemotingMessageHandler
    {
        private readonly IServiceRemotingMessageHandler handler;

        private readonly ILogger logger;

        public ServiceHostRemotingListenerMessageHandler(
            IServiceRemotingMessageHandler handler,
            ILogger logger)
        {
            this.handler = handler 
             ?? throw new ArgumentNullException(nameof(handler));

            this.logger = logger 
             ?? throw new ArgumentNullException(nameof(logger));
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
        {
            return this.handler.GetRemotingMessageBodyFactory();
        }

        public void HandleOneWayMessage(
            IServiceRemotingRequestMessage requestMessage)
        {
            var state = new ServiceHostRemotingListenerLoggerMessageState(requestMessage);
            using (this.logger.BeginScope(state))
            {
                this.handler.HandleOneWayMessage(requestMessage);
            }
        }

        public Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage)
        {
            var state = new ServiceHostRemotingListenerLoggerMessageState(requestMessage);
            using (this.logger.BeginScope(state))
            {
                return this.handler.HandleRequestResponseAsync(requestContext, requestMessage);
            }
        }
    }
}