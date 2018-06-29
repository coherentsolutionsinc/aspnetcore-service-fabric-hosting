using System;

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
            ServiceHostAspNetCoreCommunicationListenerFactory factoryFunc);

        void UseWebHostBuilder(
            Func<IWebHostBuilder> factoryFunc);

        void ConfigureWebHost(
            Action<IWebHostBuilder> configAction);
    }
}