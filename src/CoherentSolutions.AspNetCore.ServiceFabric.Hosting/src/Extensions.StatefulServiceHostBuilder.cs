using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static IStatefulServiceHostBuilder UseAspNetCoreListenerReplicaTemplate(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.UseAspNetCoreListenerReplicaTemplate(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder UseRemotingListenerReplicaTemplate(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostRemotingListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.UseRemotingListenerReplicaTemplate(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder UseListenerReplicator(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseListenerReplicator(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostBuilder Configure(
            this IStatefulServiceHostBuilder @this,
            Action<IStatefulServiceHostBuilderConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }

        public static IStatefulServiceHostBuilder DefineAspNetCoreListener(
            this IStatefulServiceHostBuilder @this,
            Action<IStatefulServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineAspNetCoreListener(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder DefineRemotingListener(
            this IStatefulServiceHostBuilder @this,
            Action<IStatefulServiceHostRemotingListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineRemotingListener(configAction));

            return @this;
        }
    }
}