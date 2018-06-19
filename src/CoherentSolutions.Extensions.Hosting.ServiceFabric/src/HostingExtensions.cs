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

        private static IHostBuilder ConfigureReliableService<TBuilder>(
            this IHostBuilder @this,
            string buildersKey,
            Func<TBuilder> buildersFactory,
            Action<TBuilder> buildersConfigAction)

        where TBuilder : IServiceHostBuilder<IServiceHost, IServiceHostBuilderConfigurator>
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (buildersKey == null)
            {
                throw new ArgumentNullException(nameof(buildersKey));
            }

            if (buildersFactory == null)
            {
                throw new ArgumentNullException(nameof(buildersFactory));
            }

            if (buildersConfigAction == null)
            {
                throw new ArgumentNullException(nameof(buildersConfigAction));
            }

            TBuilder builder;
            if (@this.Properties.TryGetValue(buildersKey, out var value))
            {
                builder = (TBuilder) value;
            }
            else
            {
                // This variable would get captured in a closure below to ensure
                // that it wouldn't be modified or obtained by anyone else.
                builder = buildersFactory();

                @this.Properties[buildersKey] = builder;
                @this.ConfigureServices(
                    services =>
                    {
                        // services variable would get captured in a closure below.
                        services.AddSingleton(provider =>
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

                            return builder.Build();
                        });
                        services.AddSingleton<IHostedService, HostingService>();
                    });
            }

            buildersConfigAction(builder);

            return @this;
        }

        public static IHostBuilder ConfigureStatefulService(
            this IHostBuilder @this,
            Action<IStatefulServiceHostBuilder> configAction)
        {
            var key = HostingDefaults.STATEFUL_BUILDER;
            var factoryFunc = new Func<IStatefulServiceHostBuilder>(() => new StatefulServiceHostBuilder());
            return @this.ConfigureReliableService(key, factoryFunc, configAction);
        }

        public static IHostBuilder ConfigureStatelessService(
            this IHostBuilder @this,
            Action<IStatelessServiceHostBuilder> configAction)
        {
            var key = HostingDefaults.STATELESS_BUILDER;
            var factoryFunc = new Func<IStatelessServiceHostBuilder>(() => new StatelessServiceHostBuilder());
            return @this.ConfigureReliableService(key, factoryFunc, configAction);
        }
    }
}
