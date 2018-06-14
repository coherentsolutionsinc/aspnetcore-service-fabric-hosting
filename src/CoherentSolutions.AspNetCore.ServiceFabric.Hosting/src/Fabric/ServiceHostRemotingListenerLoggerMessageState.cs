using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
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