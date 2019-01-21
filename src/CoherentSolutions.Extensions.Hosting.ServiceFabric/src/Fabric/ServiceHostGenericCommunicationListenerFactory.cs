using System;
using System.Fabric;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public delegate ICommunicationListener ServiceHostGenericCommunicationListenerFactory(
        ServiceContext serviceContext,
        string endpointName,
        IServiceProvider provider);
}