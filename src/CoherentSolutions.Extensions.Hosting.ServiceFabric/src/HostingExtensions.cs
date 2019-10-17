using System;
using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric
{
    public static class HostingExtensions
    {
        private static class StatelessServiceDefaults
        {
            public static IServiceCollection Dependencies()
            {
                return new ServiceCollection();
            }

            public static IServiceEventSource EventSource(
                IServiceProvider serviceProvider)
            {
                if (serviceProvider is null)
                {
                    throw new ArgumentNullException(nameof(serviceProvider));
                }

                return ActivatorUtilities.CreateInstance<ServiceEventSource>(serviceProvider);
            }

            public static IStatelessServiceRuntimeRegistrant RuntimeRegistrant()
            {
                return new StatelessServiceRuntimeRegistrant();
            }

            public static IStatelessServiceHostDelegateReplicator DelegateReplicator(
                IStatelessServiceHostDelegateReplicableTemplate template)
            {
                if (template is null)
                {
                    throw new ArgumentNullException(nameof(template));
                }

                return new StatelessServiceHostDelegateReplicator(template);
            }

            public static IStatelessServiceHostAspNetCoreListenerReplicaTemplate AspNetCoreListenerReplicaTemplate()
            {
                return new StatelessServiceHostAspNetCoreListenerReplicaTemplate();
            }

            public static IStatelessServiceHostRemotingListenerReplicaTemplate RemotingListenerReplicaTemplate()
            {
                return new StatelessServiceHostRemotingListenerReplicaTemplate();
            }

            public static IStatelessServiceHostGenericListenerReplicaTemplate GenericListenerReplicaTemplate()
            {
                return new StatelessServiceHostGenericListenerReplicaTemplate();
            }

            public static IStatelessServiceHostListenerReplicator ListenerReplicator(
                IStatelessServiceHostListenerReplicableTemplate template)
            {
                if (template is null)
                {
                    throw new ArgumentNullException(nameof(template));
                }

                return new StatelessServiceHostListenerReplicator(template);
            }
        }

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
            return @this.DefineReliableService(
                () => new StatelessServiceHostBuilder(),
                builder =>
                {
                    builder.ConfigureObject(
                        configurator =>
                        {
                            configurator.UseDependencies(StatelessServiceDefaults.Dependencies);
                            configurator.UseRuntimeRegistrant(StatelessServiceDefaults.RuntimeRegistrant);
                            
                            // Configure Event Source support
                            configurator.UseEventSourceReplicaTemplate(() =>
                            {
                                var template = new StatelessServiceHostEventSourceReplicaTemplate();
                                template.ConfigureObject(
                                    cfg =>
                                    {
                                        cfg.UseDependencies(() => new ServiceCollection());
                                        cfg.UseImplementation(provider => ActivatorUtilities.CreateInstance<ServiceEventSource>(provider));
                                    });

                                return template;
                            });
                            configurator.UseEventSourceReplicator(template => new StatelessServiceHostEventSourceReplicator(template));

                            // Configure Delegates support
                            configurator.UseDelegateReplicaTemplate(() =>
                            {
                                var template = new StatelessServiceHostDelegateReplicaTemplate();
                                template.ConfigureObject(
                                    cfg =>
                                    {
                                        cfg.UseDelegateInvoker(provider => ActivatorUtilities.CreateInstance<ServiceDelegateInvoker>(provider));
                                        cfg.UseDependencies(() => new ServiceCollection());
                                        cfg.UseLoggerOptions(() => ServiceHostLoggerOptions.Disabled);
                                    });
                                return template;
                            });
                            configurator.UseDelegateReplicator(template => new StatelessServiceHostDelegateReplicator(template));

                            // Configure Listeners support
                            configurator.UseAspNetCoreListenerReplicaTemplate(() =>
                            {
                                var template = new StatelessServiceHostAspNetCoreListenerReplicaTemplate();
                                template.ConfigureObject(
                                    cfg =>
                                    {
                                        cfg.UseEndpoint(string.Empty);
                                        cfg.UseCommunicationListener(
                                            (serviceContext, endpointName, build) =>
                                            {
                                                var @delegate = new Func<string, AspNetCoreCommunicationListener, IWebHost>(build);
                                                return new KestrelCommunicationListener(serviceContext, endpointName, @delegate);
                                            },
                                            builder => builder.UseKestrel());

                                        cfg.UseLoggerOptions(() => ServiceHostLoggerOptions.Disabled);
                                    });
                                return template;
                            });
                            configurator.UseRemotingListenerReplicaTemplate(() =>
                            {
                                var template = new StatelessServiceHostRemotingListenerReplicaTemplate();
                                template.ConfigureObject(
                                    cfg =>
                                    {
                                        cfg.UseEndpoint(string.Empty);
                                    });
                                return template;
                            });
                            configurator.UseGenericListenerReplicaTemplate(() =>
                            {
                                var template = new StatelessServiceHostGenericListenerReplicaTemplate();
                                template.ConfigureObject(
                                    cfg =>
                                    {
                                        cfg.UseEndpoint(string.Empty);

                                        cfg.UseLoggerOptions(() => ServiceHostLoggerOptions.Disabled);
                                    });
                                return template;
                            });
                            configurator.UseListenerReplicator(template => new StatelessServiceHostListenerReplicator(template));
                        });

                    // still experimental
                    var useGhost = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Fabric_ApplicationName"));
                    if (useGhost)
                    {
                        builder.ConfigureObject(
                            configurator =>
                            {
                                configurator.UseRuntimeRegistrant(
                                    () => new GhostStatelessServiceRuntimeRegistrant());
                            });
                    }
                });
        }

        private static IHostBuilder DefineReliableService<TBuilder>(
            this IHostBuilder @this,
            Func<TBuilder> buildersFactory,
            Action<TBuilder> buildersConfigAction)
            where TBuilder : IServiceHostBuilder<IServiceHost, IServiceHostBuilderConfigurator>
        {
            if (@this is null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (buildersFactory is null)
            {
                throw new ArgumentNullException(nameof(buildersFactory));
            }

            if (buildersConfigAction is null)
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
                                            // We should ignore certain types. See https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues/30
                                            dependencies.Proxinate(services, provider, typeof(IHostedService));
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