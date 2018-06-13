namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceHostRemotingListenerInformation : ServiceHostListenerInformation, IServiceHostRemotingListenerInformation
    {
        public ServiceHostRemotingListenerInformation(
            string endpointName)
            : base(endpointName)
        {
        }
    }
}