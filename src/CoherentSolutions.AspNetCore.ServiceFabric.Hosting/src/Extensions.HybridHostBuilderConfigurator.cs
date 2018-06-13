using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static TCaller UseStatefulServiceHostBuilder<TCaller>(
            this TCaller @this,
            Func<IStatefulServiceHostBuilder> factoryFunc)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseStatefulServiceHostBuilder(factoryFunc));

            return @this;
        }

        public static TCaller UseStatelessServiceHostBuilder<TCaller>(
            this TCaller @this,
            Func<IStatelessServiceHostBuilder> factoryFunc)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseStatelessServiceHostBuilder(factoryFunc));

            return @this;
        }

        public static TCaller UseHostSelector<TCaller>(
            this TCaller @this,
            Func<IHostSelector> factoryFunc)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseHostSelector(factoryFunc));

            return @this;
        }

        public static TCaller UseHost<TCaller>(
            this TCaller @this,
            Func<IHostRunner, IHost> factoryFunc)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseHost(factoryFunc));

            return @this;
        }

        public static IHybridHostBuilder Configure(
            this IHybridHostBuilder @this,
            Action<IHybridHostBuilderConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }

        public static TCaller ConfigureStatefulServiceHost<TCaller>(
            this TCaller @this,
            Action<IStatefulServiceHostBuilder> configAction)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureStatefulServiceHost(configAction));

            return @this;
        }

        public static TCaller ConfigureStatelessServiceHost<TCaller>(
            this TCaller @this,
            Action<IStatelessServiceHostBuilder> configAction)
            where TCaller : IConfigurableObject<IHybridHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureStatelessServiceHost(configAction));

            return @this;
        }
    }
}