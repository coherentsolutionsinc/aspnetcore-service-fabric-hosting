using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostRemotingListenerLoggerMessageState
    {
        public int MethodId { get; }

        public int InterfaceId { get; }

        public ServiceHostRemotingListenerLoggerMessageState(
            IServiceRemotingRequestMessage requestMessage)
        {
            var headers = requestMessage.GetHeader();

            this.MethodId = headers.MethodId;
            this.InterfaceId = headers.InterfaceId;
        }

        public override string ToString()
        {
            return $"InterfaceId: {this.InterfaceId} MethodId: {this.MethodId}";
        }
    }
}