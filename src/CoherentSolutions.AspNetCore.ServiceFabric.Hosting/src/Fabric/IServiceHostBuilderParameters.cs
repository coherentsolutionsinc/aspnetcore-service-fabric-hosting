using System;
using System.Collections.Generic;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderParameters
    {
        string ServiceName { get; }

        List<IServiceHostListenerDescriptor> ListenerDescriptors { get; }

        Func<IWebHostBuilderExtensionsImpl> WebHostBuilderExtensionsImplFunc { get; }

        Func<IWebHostExtensionsImpl> WebHostExtensionsImplFunc { get; }

        Func<IWebHostBuilder> WebHostBuilderFunc { get; }

        Action<IWebHostBuilder> WebHostConfigAction { get; }
    }
}