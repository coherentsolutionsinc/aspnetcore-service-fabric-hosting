using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
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