using System;
using System.Fabric;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
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
    }
}