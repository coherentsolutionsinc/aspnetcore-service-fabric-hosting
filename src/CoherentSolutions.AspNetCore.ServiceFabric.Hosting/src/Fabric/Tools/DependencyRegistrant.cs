using System;
using System.Fabric;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric.Tools
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
            IServiceCollection serviceCollection,
            IServiceCollection proxyServiceCollection,
            IServiceProvider proxyServiceProvider)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (proxyServiceCollection == null)
            {
                throw new ArgumentNullException(nameof(proxyServiceCollection));
            }

            if (proxyServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(proxyServiceProvider));
            }

            foreach (var descriptor in proxyServiceCollection)
            {
                switch (descriptor.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        if (descriptor.ImplementationInstance != null)
                        {
                            serviceCollection.Add(descriptor);
                        }
                        else if (descriptor.ImplementationFactory != null)
                        {
                            serviceCollection.Add(descriptor);
                        }
                        else
                        {
                            serviceCollection.Add(
                                new ServiceDescriptor(
                                    descriptor.ServiceType,
                                    provider => proxyServiceProvider.GetService(descriptor.ServiceType),
                                    ServiceLifetime.Singleton));
                        }
                        break;
                    case ServiceLifetime.Scoped:
                    case ServiceLifetime.Transient:
                        serviceCollection.Add(descriptor);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(proxyServiceCollection), descriptor.Lifetime, typeof(ServiceLifetime).FullName);
                }
            }
        }
    }
}