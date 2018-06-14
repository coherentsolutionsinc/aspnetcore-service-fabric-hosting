using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static TCaller UseWebHostBuilder<TCaller>(
            this TCaller @this,
            Func<IWebHostBuilder> factoryFunc)
            where TCaller : IConfigurableObject<IConfigurableObjectWebHostConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseWebHostBuilder(factoryFunc));

            return @this;
        }

        public static TCaller ConfigureWebHost<TCaller>(
            this TCaller @this,
            Action<IWebHostBuilder> configAction)
            where TCaller : IConfigurableObject<IConfigurableObjectWebHostConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureWebHost(configAction));

            return @this;
        }

        public static TCaller ConfigureDefaultWebHost<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IConfigurableObjectWebHostConfigurator>
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            @this.ConfigureObject(
                configurator => configurator.ConfigureWebHost(
                    config =>
                    {
                    }));

            return @this;
        }
    }
}