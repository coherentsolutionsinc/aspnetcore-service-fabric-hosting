namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
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