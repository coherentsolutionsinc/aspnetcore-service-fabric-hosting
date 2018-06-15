using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostAspNetCoreListenerInformation : ServiceHostListenerInformation, IServiceHostAspNetCoreListenerInformation
    {
        public string UrlSuffix { get; }

        public ServiceHostAspNetCoreListenerInformation(
            string endpointName,
            string urlSuffix)
            : base(endpointName)
        {
            this.UrlSuffix = urlSuffix
             ?? throw new ArgumentNullException(nameof(urlSuffix));
        }
    }
}