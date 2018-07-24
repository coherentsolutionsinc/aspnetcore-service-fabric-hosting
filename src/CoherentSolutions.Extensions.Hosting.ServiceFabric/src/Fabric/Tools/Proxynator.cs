using System;
using System.Collections.Concurrent;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public static partial class Proxynator
    {
        private partial class DependencyInjectionProxyEmitter
        {
        }

        private partial class InstanceProxyEmitter
        {
        }

        private partial class ProxyEmitter
        {
        }

        private static readonly ConcurrentDictionary<Type, Lazy<Type>> types;

        static Proxynator()
        {
            types = new ConcurrentDictionary<Type, Lazy<Type>>();
        }

        public static Type CreateInstanceProxy(
            Type interfaceType)
        {
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException($"{interfaceType.Name} isn't an interface.");
            }

            return new InstanceProxyEmitter(interfaceType).Emit();
        }

        public static Type CreateDependencyInjectionProxy(
            Type providerType,
            Type interfaceType)
        {
            if (providerType == null)
            {
                throw new ArgumentNullException(nameof(providerType));
            }

            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            if (!typeof(IServiceProvider).IsAssignableFrom(providerType))
            {
                throw new ArgumentException($"{providerType.Name} doesn't implement {nameof(IServiceProvider)}.");
            }

            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException($"{interfaceType.Name} isn't an interface.");
            }

            return new DependencyInjectionProxyEmitter(providerType, interfaceType).Emit();
        }
    }
}