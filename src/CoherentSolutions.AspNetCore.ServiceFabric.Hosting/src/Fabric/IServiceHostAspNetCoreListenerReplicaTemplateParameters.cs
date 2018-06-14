using System;
using System.Fabric;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostAspNetCoreListenerReplicaTemplateParameters 
        : IServiceHostListenerReplicaTemplateParameters,
          IConfigurableObjectWebHostParameters
    {
        ServiceFabricIntegrationOptions IntegrationOptions { get; }

        Func<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>, AspNetCoreCommunicationListener>
            AspNetCoreCommunicationListenerFunc { get; }
    }
}