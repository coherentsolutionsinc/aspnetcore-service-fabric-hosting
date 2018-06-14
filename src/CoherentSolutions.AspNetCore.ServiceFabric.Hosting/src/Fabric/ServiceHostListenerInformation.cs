using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public abstract class ServiceHostListenerInformation : IServiceHostListenerInformation
    {
        public string EndpointName { get; }

        protected ServiceHostListenerInformation(
            string endpointName)
        {
            this.EndpointName = endpointName
             ?? throw new ArgumentNullException(nameof(endpointName));
        }
    }
}