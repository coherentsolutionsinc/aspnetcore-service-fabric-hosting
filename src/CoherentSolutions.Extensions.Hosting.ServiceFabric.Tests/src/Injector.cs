using System;
using System.Reflection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public static class Injector
    {
        private const BindingFlags PUBLIC_INSTANCE =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        private const BindingFlags NONPUBLIC_INSTANCE =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        public static void InjectProperty(
            object instance,
            string property,
            object value,
            bool nonPublic)
        {
            var flags = nonPublic
                ? NONPUBLIC_INSTANCE
                : PUBLIC_INSTANCE;

            var pi = instance.GetType().GetProperty(property, flags);
            if (pi == null)
            {
                throw new InvalidOperationException($"Cannot find property {property}");
            }

            if (pi.SetMethod == null)
            { 
                pi = pi.DeclaringType.GetProperty(property, flags);
                if (pi == null)
                {
                    throw new InvalidOperationException($"Cannot find property {property}");
                }
            }

            if (pi.SetMethod == null)
            {
                throw new InvalidOperationException($"Cannot inject value because {property} is readonly");
            }

            pi.SetValue(instance, value);
        }
    }
}