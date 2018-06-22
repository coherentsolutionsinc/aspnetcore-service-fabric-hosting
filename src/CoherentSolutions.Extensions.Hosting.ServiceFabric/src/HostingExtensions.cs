using System;
using System.Collections.Generic;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric
{
    public static class HostingExtensions
    {
        private static readonly HashSet<Type> dontPropagateTypes = new HashSet<Type>
        {
            typeof(IHostedService) // For a reason see https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues/30
        };

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
            var factoryFunc = new Func<IStatelessServiceHostBuilder>(() => new StatelessServiceHostBuilder());
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
                                            DependencyRegistrant.Register(
                                                dependencies,
                                                services,
                                                provider,
                                                type => !dontPropagateTypes.Contains(type));
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