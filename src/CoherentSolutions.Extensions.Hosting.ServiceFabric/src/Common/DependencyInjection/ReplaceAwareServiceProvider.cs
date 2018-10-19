using System;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public class ReplaceAwareServiceProvider : IServiceProvider
    {
        private readonly IReadOnlyDictionary<Type, object> replacements;

        private readonly IServiceProvider impl;

        public ReplaceAwareServiceProvider(
            IDictionary<Type, object> replacements,
            IServiceProvider impl)
        {
            this.replacements = new Dictionary<Type, object>(replacements)
            {
                [typeof(IServiceProvider)] = this
            };

            this.impl = impl;
        }

        public object GetService(
            Type serviceType)
        {
            return this.replacements.TryGetValue(serviceType, out var instance)
                ? instance
                : this.impl.GetService(serviceType);
        }
    }
}