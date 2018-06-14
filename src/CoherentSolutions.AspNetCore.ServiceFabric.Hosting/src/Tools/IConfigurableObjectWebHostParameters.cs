using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools
{
    public interface IConfigurableObjectWebHostParameters
    {
        Func<IWebHostBuilderExtensionsImpl> WebHostBuilderExtensionsImplFunc { get; }

        Func<IWebHostExtensionsImpl> WebHostExtensionsImplFunc { get; }

        Func<IWebHostBuilder> WebHostBuilderFunc { get; }

        Action<IWebHostBuilder> WebHostConfigAction { get; }
    }
}