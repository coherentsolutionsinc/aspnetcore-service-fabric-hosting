using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public class OverridableServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> overrides;

        private readonly IServiceProvider impl;

        public OverridableServiceProvider(
            Dictionary<Type, object> overrides,
            IServiceProvider impl)
        {
            this.overrides = overrides;
            this.impl = impl;
        }

        public object GetService(
            Type serviceType)
        {
            if (this.overrides.TryGetValue(serviceType, out var instance))
            {
                return instance;
            }

            return this.impl.GetService(serviceType);
        }
    }
}