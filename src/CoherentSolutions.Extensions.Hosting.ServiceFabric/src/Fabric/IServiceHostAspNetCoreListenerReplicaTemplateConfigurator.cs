using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Web;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        : IServiceHostListenerReplicaTemplateConfigurator
    {
        void UseIntegrationOptions(
            ServiceFabricIntegrationOptions integrationOptions);

        void UseCommunicationListener(
            Func<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>, AspNetCoreCommunicationListener> factoryFunc);

        void UseWebHostBuilderExtensionsImpl(
            Func<IWebHostBuilderExtensionsImpl> factoryFunc);

        void UseWebHostExtensionsImpl(
            Func<IWebHostExtensionsImpl> factoryFunc);

        void UseWebHostBuilder(
            Func<IWebHostBuilder> factoryFunc);

        void ConfigureWebHost(
            Action<IWebHostBuilder> configAction);
    }
}