using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public interface IHybridHostBuilderParameters
        : IConfigurableObjectWebHostParameters
    {
        Func<IServiceHostBuilder<IServiceHost, IServiceHostBuilderConfigurator>> ServiceHostBuilderFunc { get; }

        Func<IHostSelector> HostSelectorFunc { get; }

        Func<IHostRunner, IHost> HostFunc { get; }

        Action<IServiceHostBuilder<IServiceHost, IServiceHostBuilderConfigurator>> ServiceHostConfigAction { get; }
    }
}