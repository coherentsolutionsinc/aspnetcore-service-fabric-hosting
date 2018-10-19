using System;
using System.Fabric;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
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

            var type = serviceEventSource.GetType();

            foreach (var @interface in type
               .GetInterfaces()
               .Where(@interface => typeof(IServiceEventSourceInterface).IsAssignableFrom(@interface)))
            {
                @this.Add(new ServiceDescriptor(@interface, serviceEventSource));
            }

            @this.Add(new ServiceDescriptor(type, serviceEventSource));
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
    }
}