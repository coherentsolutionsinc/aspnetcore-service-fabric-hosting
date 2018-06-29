using System;
using System.Fabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public delegate AspNetCoreCommunicationListener ServiceHostAspNetCoreCommunicationListenerFactory(
        ServiceContext serviceContext,
        string endpointName,
        Func<string, AspNetCoreCommunicationListener, IWebHost> webHostFactory);
}