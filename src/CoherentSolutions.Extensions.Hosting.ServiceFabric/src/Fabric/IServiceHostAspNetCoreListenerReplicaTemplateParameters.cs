using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAspNetCoreListenerReplicaTemplateParameters
        : IServiceHostListenerReplicaTemplateParameters
    {
        ServiceFabricIntegrationOptions IntegrationOptions { get; }

        ServiceHostAspNetCoreCommunicationListenerFactory AspNetCoreCommunicationListenerFunc { get; }

        Func<IWebHostBuilder> WebHostBuilderFunc { get; }

        Action<IWebHostBuilder> WebHostConfigAction { get; }

        Action<IWebHostBuilder> WebHostCommunicationListenerConfigAction { get; }
    }
}