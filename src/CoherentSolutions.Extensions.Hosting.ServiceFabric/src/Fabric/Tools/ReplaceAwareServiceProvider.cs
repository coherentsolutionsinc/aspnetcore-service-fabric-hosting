using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public class ReplaceAwareServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> replacements;

        private readonly IServiceProvider impl;

        public ReplaceAwareServiceProvider(
            Dictionary<Type, object> replacements,
            IServiceProvider impl)
        {
            this.replacements = replacements;
            this.impl = impl;
        }

        public object GetService(
            Type serviceType)
        {
            if (this.replacements.TryGetValue(serviceType, out var instance))
            {
                return instance;
            }

            return this.impl.GetService(serviceType);
        }
    }
}