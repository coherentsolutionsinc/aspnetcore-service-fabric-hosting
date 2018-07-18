using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public class OpenGenericAwareServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider impl;

        public OpenGenericAwareServiceProvider(
            IServiceProvider impl)
        {
            this.impl = impl ?? throw new ArgumentNullException(nameof(impl));
        }

        public object GetService(
            Type serviceType)
        {
            var service = this.impl.GetService(serviceType);
            if (service is IOpenGenericProxy proxy)
            {
                return proxy.Target;
            }

            return service;
        }
    }
}