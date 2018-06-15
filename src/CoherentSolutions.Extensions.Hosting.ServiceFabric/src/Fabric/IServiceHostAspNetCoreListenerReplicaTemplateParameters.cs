using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Web;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAspNetCoreListenerReplicaTemplateParameters
        : IServiceHostListenerReplicaTemplateParameters
    {
        ServiceFabricIntegrationOptions IntegrationOptions { get; }

        Func<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>, AspNetCoreCommunicationListener>
            AspNetCoreCommunicationListenerFunc { get; }

        Func<IWebHostBuilderExtensionsImpl> WebHostBuilderExtensionsImplFunc { get; }

        Func<IWebHostExtensionsImpl> WebHostExtensionsImplFunc { get; }

        Func<IWebHostBuilder> WebHostBuilderFunc { get; }

        Action<IWebHostBuilder> WebHostConfigAction { get; }
    }
}