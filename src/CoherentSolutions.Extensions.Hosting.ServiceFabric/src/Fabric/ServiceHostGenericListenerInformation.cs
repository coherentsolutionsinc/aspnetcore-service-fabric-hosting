namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostGenericListenerInformation : ServiceHostListenerInformation, IServiceHostGenericListenerInformation
    {
        public ServiceHostGenericListenerInformation(
            string endpointName)
            : base(endpointName)
        {
        }
    }
}