using System;
using System.Fabric;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    public static class DependencyRegistrant
    {
        public static void Register(
            IServiceCollection serviceCollection,
            ServiceContext serviceContext)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(ServiceContext), serviceContext));

            switch (serviceContext)
            {
                case StatefulServiceContext _:
                    serviceCollection.Add(new ServiceDescriptor(typeof(StatefulServiceContext), serviceContext));
                    break;
                case StatelessServiceContext _:
                    serviceCollection.Add(new ServiceDescriptor(typeof(StatelessServiceContext), serviceContext));
                    break;
            }
        }

        public static void Register(
            IServiceCollection serviceCollection,
            IServicePartition servicePartition)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (servicePartition == null)
            {
                throw new ArgumentNullException(nameof(servicePartition));
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(IServicePartition), servicePartition));

            switch (servicePartition)
            {
                case IStatefulServicePartition _:
                    serviceCollection.Add(new ServiceDescriptor(typeof(IStatefulServicePartition), servicePartition));
                    break;
                case IStatelessServicePartition _:
                    serviceCollection.Add(new ServiceDescriptor(typeof(IStatelessServicePartition), servicePartition));
                    break;
            }
        }

        public static void Register(
            IServiceCollection serviceCollection,
            IServiceEventSource serviceEventSource)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (serviceEventSource == null)
            {
                throw new ArgumentNullException(nameof(serviceEventSource));
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(IServiceEventSource), serviceEventSource));
        }

        public static void Register(
            IServiceCollection serviceCollection,
            IServiceHostAspNetCoreListenerInformation aspNetCoreListenerInformation)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (aspNetCoreListenerInformation == null)
            {
                throw new ArgumentNullException(nameof(aspNetCoreListenerInformation));
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(IServiceHostListenerInformation), aspNetCoreListenerInformation));
            serviceCollection.Add(new ServiceDescriptor(typeof(IServiceHostAspNetCoreListenerInformation), aspNetCoreListenerInformation));
        }

        public static void Register(
            IServiceCollection serviceCollection,
            IServiceHostRemotingListenerInformation remotingListenerInformation)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (remotingListenerInformation == null)
            {
                throw new ArgumentNullException(nameof(remotingListenerInformation));
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(IServiceHostListenerInformation), remotingListenerInformation));
            serviceCollection.Add(new ServiceDescriptor(typeof(IServiceHostRemotingListenerInformation), remotingListenerInformation));
        }

        public static void Register(
            IServiceCollection destination,
            IServiceCollection source,
            IServiceProvider services,
            Func<Type, bool> predicate = null)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (predicate == null)
            {
                predicate = type => true;
            }

            foreach (var descriptor in source.Where(i => predicate(i.ServiceType)))
            {
                switch (descriptor.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        if (descriptor.ImplementationInstance != null || descriptor.ImplementationFactory != null)
                        {
                            destination.Add(descriptor);
                        }
                        else
                        {
                            if (descriptor.ServiceType.GetTypeInfo().IsGenericTypeDefinition)
                            {
                                // We have open generic here. Register as-is.
                                destination.Add(descriptor);
                            }
                            else
                            {
                                destination.Add(
                                    new ServiceDescriptor(
                                        descriptor.ServiceType,
                                        provider => services.GetService(descriptor.ServiceType),
                                        ServiceLifetime.Singleton));
                            }
                        }

                        break;
                    case ServiceLifetime.Scoped:
                    case ServiceLifetime.Transient:
                        destination.Add(descriptor);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(source), descriptor.Lifetime, typeof(ServiceLifetime).FullName);
                }
            }
        }
    }
}