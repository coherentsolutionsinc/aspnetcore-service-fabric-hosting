using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static IStatelessServiceHostBuilder UseAspNetCoreListenerReplicaTemplate(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.UseAspNetCoreListenerReplicaTemplate(configAction));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseRemotingListenerReplicaTemplate(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostRemotingListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.UseRemotingListenerReplicaTemplate(configAction));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseListenerReplicator(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseListenerReplicator(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostBuilder Configure(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostBuilderConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }

        public static IStatelessServiceHostBuilder DefineAspNetCoreListener(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineAspNetCoreListener(configAction));

            return @this;
        }

        public static IStatelessServiceHostBuilder DefineRemotingListener(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostRemotingListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineRemotingListener(configAction));

            return @this;
        }
    }
}