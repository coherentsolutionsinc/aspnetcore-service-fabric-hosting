using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        : IServiceHostListenerReplicaTemplateConfigurator,
          IConfigurableObjectLoggerConfigurator
    {
        void UseIntegrationOptions(
            ServiceFabricIntegrationOptions integrationOptions);

        void UseCommunicationListener(
            ServiceHostAspNetCoreCommunicationListenerFactory factoryFunc,
            Action<IWebHostBuilder> configAction);

        void UseWebHostBuilder(
            Func<IWebHostBuilder> factoryFunc);

        void ConfigureWebHost(
            Action<IWebHostBuilder> configAction);
    }
}