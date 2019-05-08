using System;
using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric
{
    public static class HostingExtensions
    {
        public static IHostBuilder DefineStatefulService(
            this IHostBuilder @this,
            Action<IStatefulServiceHostBuilder> configAction)
        {
            var factoryFunc = new Func<IStatefulServiceHostBuilder>(() => new StatefulServiceHostBuilder());
            return @this.DefineReliableService(factoryFunc, configAction);
        }

        public static IHostBuilder DefineStatelessService(
            this IHostBuilder @this,
            Action<IStatelessServiceHostBuilder> configAction)
        {
            var factoryFunc = new Func<IStatelessServiceHostBuilder>(
                () =>
                {
                    var builder = new StatelessServiceHostBuilder();
                    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Fabric_ApplicationName")))
                    {
                        builder.ConfigureObject(
                            configurator =>
                            {
                                configurator.UseRuntimeRegistrant(
                                    () => new StatelessServiceHostedRuntimeRegistrant());
                            });
                    }

                    return builder;
                });
            return @this.DefineReliableService(factoryFunc, configAction);
        }

        private static IHostBuilder DefineReliableService<TBuilder>(
            this IHostBuilder @this,
            Func<TBuilder> buildersFactory,
            Action<TBuilder> buildersConfigAction)
            where TBuilder : IServiceHostBuilder<IServiceHost, IServiceHostBuilderConfigurator>
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (buildersFactory == null)
            {
                throw new ArgumentNullException(nameof(buildersFactory));
            }

            if (buildersConfigAction == null)
            {
                throw new ArgumentNullException(nameof(buildersConfigAction));
            }

            var builder = buildersFactory();
            @this.ConfigureServices(
                services =>
                {
                    // services variable would get captured in a closure below.
                    services.AddSingleton<IHostedService>(
                        provider =>
                        {
                            builder.ConfigureObject(
                                configurator =>
                                {
                                    configurator.ConfigureDependencies(
                                        dependencies =>
                                        {
                                            // We should ignore certain types.
                                            var ignore = new HashSet<Type>(dependencies.Select(i => i.ServiceType))
                                            {

                                                // See https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues/30
                                                typeof(IHostedService)
                                            };

                                            dependencies.Proxinate(services, provider, type => !ignore.Contains(type));
                                        });
                                });

                            return new HostingService(builder.Build());
                        });
                });

            buildersConfigAction(builder);

            return @this;
        }
    }
}