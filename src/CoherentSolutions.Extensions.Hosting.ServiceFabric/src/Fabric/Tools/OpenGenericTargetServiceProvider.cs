using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public class OpenGenericTargetServiceProvider : IOpenGenericTargetServiceProvider
    {
        private readonly IServiceProvider impl;

        public OpenGenericTargetServiceProvider(
            IServiceProvider outerServiceProviderImplementation)
        {
            this.impl = outerServiceProviderImplementation;
        }

        public object GetService(
            Type serviceType)
        {
            return this.impl.GetService(serviceType);
        }
    }
}