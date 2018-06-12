using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public abstract class ServiceListenerInformation : IServiceListenerInformation
    {
        public string EndpointName { get; }

        protected ServiceListenerInformation(
            string endpointName)
        {
            this.EndpointName = endpointName
             ?? throw new ArgumentNullException(nameof(endpointName));
        }
    }
}