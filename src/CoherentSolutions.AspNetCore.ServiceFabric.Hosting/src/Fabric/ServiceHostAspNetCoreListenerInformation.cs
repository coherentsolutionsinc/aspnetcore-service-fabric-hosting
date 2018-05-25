using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceHostAspNetCoreListenerInformation : IServiceHostAspNetCoreListenerInformation
    {
        public string EndpointName { get; }

        public string UrlSuffix { get; }

        public ServiceHostAspNetCoreListenerInformation(
            string endpointName,
            string urlSuffix)
        {
            this.EndpointName = endpointName 
             ?? throw new ArgumentNullException(nameof(endpointName));

            this.UrlSuffix = urlSuffix 
             ?? throw new ArgumentNullException(nameof(urlSuffix));
        }
    }
}