using System;
using System.Fabric;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public static class ServiceHostDependencyRegistrant
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
            IServiceHostAspNetCoreListenerInformation listenerInformation)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (listenerInformation == null)
            {
                throw new ArgumentNullException(nameof(listenerInformation));
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(IServiceHostListenerInformation), listenerInformation));
            serviceCollection.Add(new ServiceDescriptor(typeof(IServiceHostAspNetCoreListenerInformation), listenerInformation));
        }

        public static void Register(
            IServiceCollection serviceCollection,
            IServiceHostRemotingListenerInformation listenerInformation)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (listenerInformation == null)
            {
                throw new ArgumentNullException(nameof(listenerInformation));
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(IServiceHostListenerInformation), listenerInformation));
            serviceCollection.Add(new ServiceDescriptor(typeof(IServiceHostRemotingListenerInformation), listenerInformation));
        }
    }
}