using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public class ServiceAspNetCoreListenerInformation : ServiceListenerInformation, IServiceAspNetCoreListenerInformation
    {
        public string UrlSuffix { get; }

        public ServiceAspNetCoreListenerInformation(
            string endpointName,
            string urlSuffix)
            : base(endpointName)
        {
            this.UrlSuffix = urlSuffix
             ?? throw new ArgumentNullException(nameof(urlSuffix));
        }
    }
}