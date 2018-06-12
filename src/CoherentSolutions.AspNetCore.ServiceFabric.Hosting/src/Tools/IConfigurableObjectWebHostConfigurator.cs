using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools
{
    public interface IConfigurableObjectWebHostConfigurator
    {
        void UseWebHostBuilderExtensionsImpl(
            Func<IWebHostBuilderExtensionsImpl> factoryFunc);

        void UseWebHostExtensionsImpl(
            Func<IWebHostExtensionsImpl> factoryFunc);

        void UseWebHostBuilder(
            Func<IWebHostBuilder> factoryFunc);

        void ConfigureWebHost(
            Action<IWebHostBuilder> configAction);
    }
}