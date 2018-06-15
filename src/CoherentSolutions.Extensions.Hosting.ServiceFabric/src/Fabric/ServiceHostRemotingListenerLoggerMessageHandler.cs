using System.Fabric;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostRemotingListenerLoggerMessageHandler : ServiceRemotingMessageDispatcher
    {
        private readonly ILogger logger;

        public ServiceHostRemotingListenerLoggerMessageHandler(
            ServiceContext serviceContext,
            Microsoft.ServiceFabric.Services.Remoting.IService serviceImplementation,
            ILogger logger)
            : base(serviceContext, serviceImplementation)
        {
            this.logger = logger;
        }

        public override void HandleOneWayMessage(
            IServiceRemotingRequestMessage requestMessage)
        {
            var state = new ServiceHostRemotingListenerLoggerMessageState(requestMessage);
            using (this.logger.BeginScope(state))
            {
                base.HandleOneWayMessage(requestMessage);
            }
        }

        public override Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage)
        {
            var state = new ServiceHostRemotingListenerLoggerMessageState(requestMessage);
            using (this.logger.BeginScope(state))
            {
                return base.HandleRequestResponseAsync(requestContext, requestMessage);
            }
        }
    }
}