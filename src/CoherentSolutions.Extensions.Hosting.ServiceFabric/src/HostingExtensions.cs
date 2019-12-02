using System;
using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.NodeContexts;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric
{
    public static class HostingExtensions
    {
        private enum HostingRuntime
        {
            Default,
            Local
        }

        private static class HostingConfiguration
        {
            public const string RUNTIME = "CS_EHSF_RUNTIME";
        }

        private static class HostingEnvironment
        {
            public static HostingRuntime GetRuntime()
            {
                var runtimeString = Environment.GetEnvironmentVariable(HostingConfiguration.RUNTIME);
                if (runtimeString is null || !Enum.TryParse<HostingRuntime>(runtimeString, true, out var runtime))
                {
                    return HostingRuntime.Default;
                }

                return runtime;
            }
        }

        public static IHostBuilder DefineStatefulService(
            this IHostBuilder @this,
            Action<IStatefulServiceHostBuilder> externalConfigAction)
        {
            if (@this is null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (externalConfigAction is null)
            {
                throw new ArgumentNullException(nameof(externalConfigAction));
            }

            @this.InjectReliableService<IStatefulServiceHostBuilder>(
                internalConfigAction =>
                {
                    var builder = new StatefulServiceHostBuilder();
                    builder.ConfigureObject(
                        configurator =>
                        {
                            switch (HostingEnvironment.GetRuntime())
                            {
                                case HostingRuntime.Default:
                                    configurator.ConfigureDependencies(
                                        dependencies =>
                                        {
                                            dependencies.AddSingleton<IServiceHostRuntime, ServiceHostRuntime>();
                                        });
                                    break;
                                case HostingRuntime.Local:
                                    throw new NotSupportedException("Stateful Services can't be executed using Local Runtime");
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        });

                    externalConfigAction(builder);
                    internalConfigAction(builder);

                    return builder.Build();
                });

            return @this;
        }

        public static IHostBuilder DefineStatelessService(
            this IHostBuilder @this,
            Action<IStatelessServiceHostBuilder> externalConfigAction)
        {
            if (@this is null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (externalConfigAction is null)
            {
                throw new ArgumentNullException(nameof(externalConfigAction));
            }

            @this.InjectReliableService<IStatelessServiceHostBuilder>(
                internalConfigAction =>
                {
                    var builder = new StatelessServiceHostBuilder();
                    builder.ConfigureObject(
                        configurator =>
                        {
                            switch (HostingEnvironment.GetRuntime())
                            {
                                case HostingRuntime.Default:
                                    configurator.ConfigureDependencies(
                                        dependencies =>
                                        {
                                            dependencies.AddSingleton<IServiceHostRuntime, ServiceHostRuntime>();
                                        });
                                    break;
                                case HostingRuntime.Local:
                                    configurator.ConfigureDependencies(
                                        dependencies =>
                                        {
                                            dependencies.AddLogging(logging => logging.AddConsole());

                                            dependencies.AddSingleton(
                                                p => ActivatorUtilities.CreateInstance<ServiceActivationContextProvider>(p).GetActivationContext());
                                            dependencies.AddSingleton(
                                                p => ActivatorUtilities.CreateInstance<NodeContextProvider>(p).GetNodeContext());
                                            dependencies.AddSingleton(
                                                p => ActivatorUtilities.CreateInstance<ServicePackageProvider>(p).GetPackage());
                                            dependencies.AddSingleton(
                                                p => ActivatorUtilities.CreateInstance<ServiceManifestProvider>(p).GetManifest());
                                            dependencies.AddSingleton(
                                                p => ActivatorUtilities.CreateInstance<CodePackageActivationContextProvider>(p).GetActivationContext());

                                            dependencies.AddTransient<IServiceManifestReader, ServiceManifestReader>();
                                            dependencies.AddTransient<ICodePackageActivationContextReader, CodePackageActivationContextReader>();

                                            dependencies.AddSingleton<IServiceHostRuntime, LocalRuntime>();
                                        });

                                    configurator.UseRemotingListenerReplicaTemplate(
                                        () => new LocalRuntimeStatelessRemotingListenerReplicaTemplate());
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        });

                    externalConfigAction(builder);
                    internalConfigAction(builder);

                    return builder.Build();
                });

            return @this;
        }

        private static IHostBuilder InjectReliableService<TBuilder>(
            this IHostBuilder @this,
            Func<Action<TBuilder>, IServiceHost> hostFactory)
            where TBuilder : IServiceHostBuilder<IServiceHost, IServiceHostBuilderConfigurator>
        {
            if (@this is null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (hostFactory is null)
            {
                throw new ArgumentNullException(nameof(hostFactory));
            }

            @this.ConfigureServices(
                services =>
                {
                    services.AddSingleton<IHostedService>(
                        provider =>
                        {
                            return new HostingService(
                                hostFactory(
                                    builder =>
                                    {
                                        builder.ConfigureObject(
                                            configurator =>
                                            {
                                                configurator.ConfigureDependencies(
                                                    dependencies =>
                                                    {
                                                        // We should ignore certain types.
                                                        var ignore = new HashSet<Type>(dependencies.Select(i => i.ServiceType));

                                                        // See https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues/30
                                                        ignore.Add(typeof(IHostedService));

                                                        dependencies.Proxinate(services, provider, type => !ignore.Contains(type));
                                                    });
                                            });
                                    }));
                        });
                });

            return @this;
        }
    }
}