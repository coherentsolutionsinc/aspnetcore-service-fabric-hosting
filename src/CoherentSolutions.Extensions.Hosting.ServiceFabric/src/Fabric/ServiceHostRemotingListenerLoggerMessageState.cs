using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostRemotingListenerLoggerMessageState
    {
        private readonly int methodId;

        private readonly int interfaceId;

        public ServiceHostRemotingListenerLoggerMessageState(
            IServiceRemotingRequestMessage requestMessage)
        {
            var headers = requestMessage.GetHeader();

            this.methodId = headers.MethodId;
            this.interfaceId = headers.InterfaceId;
        }

        public override string ToString()
        {
            return $"InterfaceId: {this.interfaceId} MethodId: {this.methodId}";
        }
    }
}