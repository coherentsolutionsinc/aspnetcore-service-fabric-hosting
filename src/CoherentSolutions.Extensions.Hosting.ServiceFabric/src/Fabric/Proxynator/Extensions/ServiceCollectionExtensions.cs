using System;
using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void Proxinate(
            this IServiceCollection @this,
            IServiceCollection servicesCollection,
            IServiceProvider servicesProvider,
            params Type[] forbidden)
        {
            if (@this is null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (servicesCollection is null)
            {
                throw new ArgumentNullException(nameof(servicesCollection));
            }

            if (servicesProvider is null)
            {
                throw new ArgumentNullException(nameof(servicesProvider));
            }

            if (servicesCollection.Count == 0)
            {
                return;
            }

            // We shouldn't override local registration or proxinate forbidden types.
            var local = new HashSet<Type>(@this.Select(i => i.ServiceType).Concat(forbidden));

            // In case we have open-generic types we has to generate special
            // dependency injection provider - this would be the type.
            Type providerType = null;

            foreach (var descriptor in servicesCollection.Where(i => !local.Contains(i.ServiceType)))
            {
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
                                    if (providerType is null)
                                    {
                                        providerType = Proxynator.CreateInstanceProxy(typeof(IServiceProvider));
                                    }
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

            if (providerType is object)
            {
                @this.Add(
                    new ServiceDescriptor(
                        providerType,
                        Activator.CreateInstance(providerType, new ProxynatorAwareServiceProvider(servicesProvider))));
            }
        }
    }
}