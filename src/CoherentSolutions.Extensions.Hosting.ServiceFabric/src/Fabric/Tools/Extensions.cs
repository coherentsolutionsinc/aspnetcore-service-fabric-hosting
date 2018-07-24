using System;
using System.Fabric;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    internal static class ServiceCollectionExtensions
    {
        public static void Add(
            this IServiceCollection @this,
            ServiceContext serviceContext)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }

            @this.Add(new ServiceDescriptor(typeof(ServiceContext), serviceContext));

            switch (serviceContext)
            {
                case StatefulServiceContext _:
                    @this.Add(new ServiceDescriptor(typeof(StatefulServiceContext), serviceContext));
                    break;
                case StatelessServiceContext _:
                    @this.Add(new ServiceDescriptor(typeof(StatelessServiceContext), serviceContext));
                    break;
            }
        }

        public static void Add(
            this IServiceCollection @this,
            IServicePartition servicePartition)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (servicePartition == null)
            {
                throw new ArgumentNullException(nameof(servicePartition));
            }

            @this.Add(new ServiceDescriptor(typeof(IServicePartition), servicePartition));

            switch (servicePartition)
            {
                case IStatefulServicePartition _:
                    @this.Add(new ServiceDescriptor(typeof(IStatefulServicePartition), servicePartition));
                    break;
                case IStatelessServicePartition _:
                    @this.Add(new ServiceDescriptor(typeof(IStatelessServicePartition), servicePartition));
                    break;
            }
        }

        public static void Add(
            this IServiceCollection @this,
            IServiceEventSource serviceEventSource)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (serviceEventSource == null)
            {
                throw new ArgumentNullException(nameof(serviceEventSource));
            }

            @this.Add(new ServiceDescriptor(typeof(IServiceEventSource), serviceEventSource));
        }

        public static void Add(
            this IServiceCollection @this,
            IServiceHostAspNetCoreListenerInformation aspNetCoreListenerInformation)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (aspNetCoreListenerInformation == null)
            {
                throw new ArgumentNullException(nameof(aspNetCoreListenerInformation));
            }

            @this.Add(new ServiceDescriptor(typeof(IServiceHostListenerInformation), aspNetCoreListenerInformation));
            @this.Add(new ServiceDescriptor(typeof(IServiceHostAspNetCoreListenerInformation), aspNetCoreListenerInformation));
        }

        public static void Add(
            this IServiceCollection @this,
            IServiceHostRemotingListenerInformation remotingListenerInformation)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (remotingListenerInformation == null)
            {
                throw new ArgumentNullException(nameof(remotingListenerInformation));
            }

            @this.Add(new ServiceDescriptor(typeof(IServiceHostListenerInformation), remotingListenerInformation));
            @this.Add(new ServiceDescriptor(typeof(IServiceHostRemotingListenerInformation), remotingListenerInformation));
        }

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

            var providerType = Proxynator.CreateInstanceProxy(typeof(IServiceProvider));

            @this.Add(
                new ServiceDescriptor(
                    providerType,
                    provider => Activator.CreateInstance(providerType, new ProxynatorAwareServiceProvider(services)),
                    ServiceLifetime.Singleton));

            foreach (var descriptor in collection.Where(i => predicate(i.ServiceType)))
            {
                switch (descriptor.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        if (descriptor.ImplementationInstance != null || descriptor.ImplementationFactory != null)
                        {
                            @this.Add(descriptor);
                        }
                        else
                        {
                            if (descriptor.ServiceType.GetTypeInfo().IsGenericTypeDefinition && descriptor.ServiceType.IsInterface)
                            {
                                @this.Add(
                                    new ServiceDescriptor(
                                        descriptor.ServiceType,
                                        Proxynator.CreateDependencyInjectionProxy(providerType, descriptor.ServiceType),
                                        ServiceLifetime.Singleton));
                            }
                            else
                            {
                                @this.Add(
                                    new ServiceDescriptor(
                                        descriptor.ServiceType,
                                        provider => services.GetService(descriptor.ServiceType),
                                        ServiceLifetime.Singleton));
                            }
                        }

                        break;
                    case ServiceLifetime.Scoped:
                    case ServiceLifetime.Transient:
                        @this.Add(descriptor);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(@this), descriptor.Lifetime, typeof(ServiceLifetime).FullName);
                }
            }
        }
    }
}