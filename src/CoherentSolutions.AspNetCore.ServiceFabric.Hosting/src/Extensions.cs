using System;
using System.ComponentModel;
using System.Fabric;
using System.Reflection;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static class Extensions
    {
        public static string GetDescription(
            this object @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            var type = @this.GetType();
            var attribute = type.GetCustomAttribute<DescriptionAttribute>(false);
            return attribute?.Description;
        }

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

        public static TCaller UseWebHostBuilder<TCaller>(
            this TCaller @this,
            Func<IWebHostBuilder> factoryFunc)
            where TCaller : IConfigurableObject<IConfigurableObjectWebHostConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseWebHostBuilder(factoryFunc));

            return @this;
        }

        public static TCaller UseServiceName<TCaller>(
            this TCaller @this,
            string serviceName)
            where TCaller : IConfigurableObject<IServiceHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseServiceName(serviceName));

            return @this;
        }

        public static TCaller UseEndpointName<TCaller>(
            this TCaller @this,
            string endpointName)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseEndpointName(endpointName));

            return @this;
        }

        public static TCaller UseLoggerOptions<TCaller>(
            this TCaller @this,
            Func<IServiceAspNetCoreListenerLoggerOptions> factoryFunc)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseLoggerOptions(factoryFunc));

            return @this;
        }

        public static TCaller UseStatefulServiceHostBuilder<TCaller>(
            this TCaller @this,
            Func<IStatefulServiceHostBuilder> factoryFunc)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseStatefulServiceHostBuilder(factoryFunc));

            return @this;
        }

        public static TCaller UseStatelessServiceHostBuilder<TCaller>(
            this TCaller @this,
            Func<IStatelessServiceHostBuilder> factoryFunc)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseStatelessServiceHostBuilder(factoryFunc));

            return @this;
        }

        public static TCaller UseHostSelector<TCaller>(
            this TCaller @this,
            Func<IHostSelector> factoryFunc)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseHostSelector(factoryFunc));

            return @this;
        }

        public static TCaller UseHost<TCaller>(
            this TCaller @this,
            Func<IHostRunner, IHost> factoryFunc)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseHost(factoryFunc));

            return @this;
        }

        public static TCaller UseAspNetCoreCommunicationListener<TCaller>(
            this TCaller @this,
            Func<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>, AspNetCoreCommunicationListener> factoryFunc)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseAspNetCoreCommunicationListener(factoryFunc));

            return @this;
        }

        public static TCaller UseKestrel<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseAspNetCoreCommunicationListener(
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

        public static IStatefulServiceHostAspNetCoreListenerReplicaTemplate UseListenerOnSecondary(
            this IStatefulServiceHostAspNetCoreListenerReplicaTemplate @this)
        {
            @this.ConfigureObject(
                configurator => configurator.UseListenerOnSecondary());

            return @this;
        }

        public static IHybridHostBuilder Configure(
            this IHybridHostBuilder @this,
            Action<IHybridHostBuilderConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }

        public static IStatefulServiceHostBuilder Configure(
            this IStatefulServiceHostBuilder @this,
            Action<IStatefulServiceHostBuilderConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }

        public static IStatelessServiceHostBuilder Configure(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostBuilderConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }

        public static TCaller ConfigureDefaultWebHost<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IConfigurableObjectWebHostConfigurator>
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            @this.ConfigureWebHost(NullWebHostConfigureImpl);

            return @this;
        }

        public static IStatefulServiceHostAspNetCoreListenerReplicaTemplate Configure(
            this IStatefulServiceHostAspNetCoreListenerReplicaTemplate @this,
            Action<IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }

        public static IStatelessServiceHostAspNetCoreListenerReplicaTemplate Configure(
            this IStatelessServiceHostAspNetCoreListenerReplicaTemplate @this,
            Action<IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }

        public static TCaller ConfigureWebHost<TCaller>(
            this TCaller @this,
            Action<IWebHostBuilder> configAction)
            where TCaller : IConfigurableObject<IConfigurableObjectWebHostConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureWebHost(configAction));

            return @this;
        }

        public static TCaller ConfigureStatefulServiceHost<TCaller>(
            this TCaller @this,
            Action<IStatefulServiceHostBuilder> configAction)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureStatefulServiceHost(configAction));

            return @this;
        }

        public static TCaller ConfigureStatelessServiceHost<TCaller>(
            this TCaller @this,
            Action<IStatelessServiceHostBuilder> configAction)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureStatelessServiceHost(configAction));

            return @this;
        }

        public static IWebHostBuilder ConfigureOnRun(
            this IWebHostBuilder @this,
            Action<IServiceProvider> onRunAction)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (onRunAction == null)
            {
                throw new ArgumentNullException(nameof(onRunAction));
            }

            if (!(@this is ExtensibleWebHostBuilder))
            {
                throw new InvalidOperationException(
                    $"Unable to configure on-run action execution because this "
                  + $"method is invoked outside of {nameof(IHybridHostBuilder)} boundaries.");
            }

            @this.ConfigureServices(
                services =>
                {
                    services.AddSingleton<IExtensibleWebHostOnRunAction>(new ExtensibleWebHostOnRunAction(onRunAction));
                });

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

        public static IStatelessServiceHostBuilder DefineAspNetCoreListener(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineAspNetCoreListener(configAction));

            return @this;
        }

        private static void NullWebHostConfigureImpl(
            IWebHostBuilder webHostBuilder)
        {
        }
    }
}