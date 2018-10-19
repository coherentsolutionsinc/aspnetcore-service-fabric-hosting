using System;
using System.Collections.Concurrent;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator
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

        private static readonly ConcurrentDictionary<Type, Type> providerTypes;

        private static readonly ConcurrentDictionary<Type, Type> interfaceTypes;

        private static readonly ConcurrentDictionary<Type, Type> implementationTypes;

        static Proxynator()
        {
            providerTypes = new ConcurrentDictionary<Type, Type>();
            interfaceTypes = new ConcurrentDictionary<Type, Type>();
            implementationTypes = new ConcurrentDictionary<Type, Type>();
        }

        public static Type GetProxyProviderType(
            Type proxyType)
        {
            return providerTypes.TryGetValue(proxyType, out var providerType)
                ? providerType
                : null;
        }

        public static Type GetProxyInterfaceType(
            Type proxyType)
        {
            return interfaceTypes.TryGetValue(proxyType, out var interfaceType)
                ? interfaceType
                : null;
        }

        public static Type GetProxyImplementationType(
            Type proxyType)
        {
            return implementationTypes.TryGetValue(proxyType, out var implementationType)
                ? implementationType
                : null;
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

            var proxyType = new InstanceProxyEmitter(interfaceType).Emit();

            interfaceTypes.TryAdd(proxyType, interfaceType);

            return proxyType;
        }

        public static Type CreateDependencyInjectionProxy(
            Type providerType,
            Type interfaceType,
            Type implementationType)
        {
            if (providerType == null)
            {
                throw new ArgumentNullException(nameof(providerType));
            }

            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (!typeof(IServiceProvider).IsAssignableFrom(providerType))
            {
                throw new ArgumentException($"{providerType.Name} doesn't implement {nameof(IServiceProvider)}.");
            }

            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException($"{interfaceType.Name} isn't an interface.");
            }

            var proxyType = new DependencyInjectionProxyEmitter(providerType, interfaceType, implementationType).Emit();

            providerTypes.TryAdd(proxyType, providerType);
            interfaceTypes.TryAdd(proxyType, interfaceType);
            implementationTypes.TryAdd(proxyType, implementationType);

            return proxyType;
        }
    }
}