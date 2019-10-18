using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

using IRemotingImplementation = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric
{
    public static class HostingExtensions
    {
        public static IHostBuilder DefineStatefulService(
            this IHostBuilder @this,
            Action<IStatefulServiceHostBuilder> configAction)
        {
            return @this.DefineReliableService(
                () => new StatefulServiceHostBuilder(),
                builder =>
                {
                    builder.ConfigureObject(
                        configurator =>
                        {
                            configurator.UseDependencies(() => new ServiceCollection());
                            configurator.UseRuntimeRegistrant(() => new StatefulServiceRuntimeRegistrant());

                            // Configure Event Source support
                            configurator.UseEventSourceReplicaTemplate(() =>
                            {
                                var template = new StatefulServiceHostEventSourceReplicaTemplate();
                                template.ConfigureObject(
                                    cfg =>
                                    {
                                        cfg.UseDependencies(() => new ServiceCollection());
                                        cfg.UseImplementation(provider => ActivatorUtilities.CreateInstance<ServiceEventSource>(provider));
                                    });

                                return template;
                            });
                            configurator.UseEventSourceReplicator(template => new StatefulServiceHostEventSourceReplicator(template));

                            // Configure Delegates support
                            configurator.UseDelegateReplicaTemplate(() =>
                            {
                                var template = new StatefulServiceHostDelegateReplicaTemplate();
                                template.ConfigureObject(
                                    cfg =>
                                    {
                                        cfg.UseDelegateInvoker(provider => ActivatorUtilities.CreateInstance<ServiceDelegateInvoker>(provider));
                                        cfg.UseDependencies(() => new ServiceCollection());
                                        cfg.UseLoggerOptions(() => ServiceHostLoggerOptions.Disabled);
                                    });
                                return template;
                            });
                            configurator.UseDelegateReplicator(template => new StatefulServiceHostDelegateReplicator(template));

                            // Configure Listeners support
                            configurator.UseAspNetCoreListenerReplicaTemplate(() =>
                            {
                                var template = new StatefulServiceHostAspNetCoreListenerReplicaTemplate();
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
                                        cfg.UseWebHostBuilder(() => WebHost.CreateDefaultBuilder());

                                        cfg.UseLoggerOptions(() => ServiceHostLoggerOptions.Disabled);
                                    });
                                return template;
                            });
                            configurator.UseRemotingListenerReplicaTemplate(() =>
                            {
                                var template = new StatefulServiceHostRemotingListenerReplicaTemplate();
                                template.ConfigureObject(
                                    cfg =>
                                    {
                                        cfg.UseEndpoint(string.Empty);
                                        cfg.UseDependencies(() => new ServiceCollection());
                                        cfg.UseCommunicationListener(
                                            (serviceContext, build) =>
                                            {
                                                var components = build(serviceContext);
                                                return new FabricTransportServiceRemotingListener(
                                                    serviceContext,
                                                    components.MessageHandler,
                                                    components.ListenerSettings,
                                                    components.MessageSerializationProvider);
                                            });
                                        cfg.UseSettings(() => new FabricTransportRemotingListenerSettings());
                                        cfg.UseHandler(provider =>
                                        {
                                            var serviceContext = provider.GetService<ServiceContext>();
                                            var serviceImplementation = provider.GetService<IRemotingImplementation>();
                                            var serviceRemotingMessageBodyFactory = provider.GetService<IServiceRemotingMessageBodyFactory>();

                                            return new ServiceRemotingMessageDispatcher(
                                                serviceContext,
                                                serviceImplementation,
                                                serviceRemotingMessageBodyFactory);
                                        });

                                        cfg.UseLoggerOptions(() => ServiceHostLoggerOptions.Disabled);
                                    });
                                return template;
                            });
                            configurator.UseGenericListenerReplicaTemplate(() =>
                            {
                                var template = new StatefulServiceHostGenericListenerReplicaTemplate();
                                template.ConfigureObject(
                                    cfg =>
                                    {
                                        cfg.UseEndpoint(string.Empty);
                                        cfg.UseDependencies(() => new ServiceCollection());

                                        cfg.UseLoggerOptions(() => ServiceHostLoggerOptions.Disabled);
                                    });
                                return template;
                            });
                            configurator.UseListenerReplicator(template => new StatefulServiceHostListenerReplicator(template));
                        });

                    configAction(builder);
                });
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
                            configurator.UseDependencies(() => new ServiceCollection());
                            configurator.UseRuntimeRegistrant(() => new StatelessServiceRuntimeRegistrant());
                            
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
                                        cfg.UseWebHostBuilder(() => WebHost.CreateDefaultBuilder());

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
                                        cfg.UseDependencies(() => new ServiceCollection());
                                        cfg.UseCommunicationListener(
                                            (serviceContext, build) =>
                                            {
                                                var components = build(serviceContext);
                                                return new FabricTransportServiceRemotingListener(
                                                    serviceContext,
                                                    components.MessageHandler,
                                                    components.ListenerSettings,
                                                    components.MessageSerializationProvider);
                                            });
                                        cfg.UseSettings(() => new FabricTransportRemotingListenerSettings());
                                        cfg.UseHandler(provider =>
                                        {
                                            var serviceContext = provider.GetService<ServiceContext>();
                                            var serviceImplementation = provider.GetService<IRemotingImplementation>();
                                            var serviceRemotingMessageBodyFactory = provider.GetService<IServiceRemotingMessageBodyFactory>();

                                            return new ServiceRemotingMessageDispatcher(
                                                serviceContext,
                                                serviceImplementation,
                                                serviceRemotingMessageBodyFactory);
                                        });

                                        cfg.UseLoggerOptions(() => ServiceHostLoggerOptions.Disabled);
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
                                        cfg.UseDependencies(() => new ServiceCollection());

                                        cfg.UseLoggerOptions(() => ServiceHostLoggerOptions.Disabled);
                                    });
                                return template;
                            });
                            configurator.UseListenerReplicator(template => new StatelessServiceHostListenerReplicator(template));
                        });

                    configAction(builder);

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