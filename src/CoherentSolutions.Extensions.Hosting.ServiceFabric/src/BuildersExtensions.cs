using System;
using System.Fabric;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Remoting.V2;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric
{
    public static class BuildersExtensions
    {
        public static Action<T> Chain<T>(
            this Action<T> left,
            Action<T> right)
        {
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            return v =>
            {
                left(v);
                right(v);
            };
        }

        public static TCaller UseServiceType<TCaller>(
            this TCaller @this,
            string serviceName)
            where TCaller : IConfigurableObject<IServiceHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseServiceType(serviceName));

            return @this;
        }

        public static TCaller UseEndpointName<TCaller>(
            this TCaller @this,
            string endpointName)
            where TCaller : IConfigurableObject<IServiceHostListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseEndpointName(endpointName));

            return @this;
        }

        public static TCaller UseCommunicationListener<TCaller>(
            this TCaller @this,
            Func<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>, AspNetCoreCommunicationListener> factoryFunc)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseCommunicationListener(factoryFunc));

            return @this;
        }

        public static TCaller UseLoggerOptions<TCaller>(
            this TCaller @this,
            Func<IServiceHostListenerLoggerOptions> factoryFunc)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseLoggerOptions(factoryFunc));

            return @this;
        }

        public static TCaller UseKestrel<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseCommunicationListener(
                    (
                        context,
                        endpoint,
                        factory) => new KestrelCommunicationListener(context, endpoint, factory)));

            return @this;
        }

        public static TCaller UseUniqueServiceUrlIntegration<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator
                    => configurator.UseIntegrationOptions(ServiceFabricIntegrationOptions.UseUniqueServiceUrl));

            return @this;
        }

        public static TCaller UseReverseProxyIntegration<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator
                    => configurator.UseIntegrationOptions(ServiceFabricIntegrationOptions.UseReverseProxyIntegration));

            return @this;
        }

        public static TCaller UseWebHostBuilder<TCaller>(
            this TCaller @this,
            Func<IWebHostBuilder> factoryFunc)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseWebHostBuilder(factoryFunc));

            return @this;
        }

        public static TCaller UseListenerOnSecondary<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IStatefulServiceHostListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseListenerOnSecondary());

            return @this;
        }

        public static IStatefulServiceHostBuilder UseAspNetCoreListenerReplicaTemplate(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.UseAspNetCoreListenerReplicaTemplate(configAction));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseAspNetCoreListenerReplicaTemplate(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.UseAspNetCoreListenerReplicaTemplate(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder UseRemotingListenerReplicaTemplate(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostRemotingListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.UseRemotingListenerReplicaTemplate(configAction));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseRemotingListenerReplicaTemplate(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostRemotingListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.UseRemotingListenerReplicaTemplate(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder UseListenerReplicator(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseListenerReplicator(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseListenerReplicator(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseListenerReplicator(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation<TRemotingImplementation>(null));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Func<TRemotingImplementation> factoryFunc)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation<TRemotingImplementation>(null));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Func<TRemotingImplementation> factoryFunc)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseSerializer<TSerializer>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this)
            where TSerializer : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializer<TSerializer>(null));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseSerializer<TSerializer>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Func<TSerializer> factoryFunc)
            where TSerializer : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializer(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseSerializer<TSerializer>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Func<TSerializer> factoryFunc)
            where TSerializer : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializer(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseSerializer<TSerializer>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this)
            where TSerializer : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializer<TSerializer>(null));

            return @this;
        }

        public static TCaller ConfigureWebHost<TCaller>(
            this TCaller @this,
            Action<IWebHostBuilder> configAction)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureWebHost(configAction));

            return @this;
        }

        public static TCaller ConfigureDependencies<TCaller>(
            this TCaller @this,
            Action<IServiceCollection> configAction)
            where TCaller : IConfigurableObject<IConfigurableObjectDependenciesConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureDependencies(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder DefineDelegate(
            this IStatefulServiceHostBuilder @this,
            Action<IStatefulServiceHostAsyncDelegateReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineAsyncDelegate(configAction));

            return @this;
        }
        
        public static IStatelessServiceHostBuilder DefineDelegate(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostAsyncDelegateReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineAsyncDelegate(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder DefineAspNetCoreListener(
            this IStatefulServiceHostBuilder @this,
            Action<IStatefulServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineAspNetCoreListener(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder DefineRemotingListener(
            this IStatefulServiceHostBuilder @this,
            Action<IStatefulServiceHostRemotingListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineRemotingListener(configAction));

            return @this;
        }

        public static IStatelessServiceHostBuilder DefineAspNetCoreListener(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineAspNetCoreListener(configAction));

            return @this;
        }

        public static IStatelessServiceHostBuilder DefineRemotingListener(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostRemotingListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineRemotingListener(configAction));

            return @this;
        }
    }
}