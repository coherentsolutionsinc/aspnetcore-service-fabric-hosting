using System;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;
using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public interface IHostBuilderParameters
    {
        Func<IWebHostBuilderExtensionsImpl> WebHostBuilderExtensionsImplFunc { get; }

        Func<IWebHostExtensionsImpl> WebHostExtensionsImplFunc { get; }

        Func<IWebHostBuilder> WebHostBuilderFunc { get; }

        Func<IServiceHostBuilder<IServiceHost, IServiceHostBuilderConfigurator>> ServiceHostBuilderFunc { get; }

        Func<IHostSelector> HostSelectorFunc { get; }

        Func<IHostRunner, IHost> HostFunc { get; }

        Action<IServiceHostBuilder<IServiceHost, IServiceHostBuilderConfigurator>> ServiceHostConfigAction { get; }

        Action<IWebHostBuilder> WebHostConfigAction { get; }
    }
}