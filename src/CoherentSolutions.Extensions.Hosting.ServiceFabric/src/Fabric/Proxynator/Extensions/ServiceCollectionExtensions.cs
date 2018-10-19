using System;
using System.Fabric;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public static class ServiceCollectionExtensions
    {
        public static void Proxinate(
            this IServiceCollection @this,
            IServiceCollection collection,
            IServiceProvider services,
            Func<Type, bool> predicate = null)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (predicate == null)
            {
                predicate = type => true;
            }

            if (collection.Count == 0)
            {
                return;
            }

            Type providerType = null;

            foreach (var descriptor in collection.Where(i => predicate(i.ServiceType)))
            {
                // Create provider only when there are new open-generics
                if (providerType == null
                 && descriptor.Lifetime == ServiceLifetime.Singleton
                 && descriptor.ServiceType.IsInterface
                 && descriptor.ImplementationType != null
                 && typeof(IProxynatorProxy).IsAssignableFrom(descriptor.ImplementationType) == false)
                {
                    providerType = Proxynator.CreateInstanceProxy(typeof(IServiceProvider));
                }

                ServiceDescriptor updateDescriptor;
                switch (descriptor.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        if (descriptor.ImplementationInstance != null || descriptor.ImplementationFactory != null)
                        {
                            updateDescriptor = descriptor;
                        }
                        else
                        {
                            if (descriptor.ServiceType.IsInterface)
                            {
                                // Reuse same descriptor for proxy registrations
                                if (typeof(IProxynatorProxy).IsAssignableFrom(descriptor.ImplementationType))
                                {
                                    updateDescriptor = descriptor;
                                }
                                else
                                {
                                    updateDescriptor =
                                        new ServiceDescriptor(
                                            descriptor.ServiceType,
                                            Proxynator.CreateDependencyInjectionProxy(
                                                providerType,
                                                descriptor.ServiceType,
                                                descriptor.ImplementationType),
                                            ServiceLifetime.Singleton);
                                }
                            }
                            else
                            {
                                // Currently open-generics base classes aren't supported.
                                updateDescriptor = descriptor;
                            }
                        }

                        break;
                    case ServiceLifetime.Scoped:
                    case ServiceLifetime.Transient:
                        updateDescriptor = descriptor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(@this), descriptor.Lifetime, typeof(ServiceLifetime).FullName);
                }

                @this.Add(updateDescriptor);
            }

            if (providerType != null)
            {
                @this.Add(
                    new ServiceDescriptor(
                        providerType,
                        Activator.CreateInstance(providerType, new ProxynatorAwareServiceProvider(services))));
            }
        }
    }
}