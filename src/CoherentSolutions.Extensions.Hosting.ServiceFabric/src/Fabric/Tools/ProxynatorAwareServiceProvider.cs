using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public class ProxynatorAwareServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider impl;

        public ProxynatorAwareServiceProvider(
            IServiceProvider impl)
        {
            this.impl = impl ?? throw new ArgumentNullException(nameof(impl));
        }

        public object GetService(
            Type serviceType)
        {
            var service = this.impl.GetService(serviceType);
            if (service is IProxynatorProxy proxy)
            {
                return proxy.Target;
            }

            return service;
        }
    }
}