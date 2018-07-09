using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Data;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features.DependencyInjection
{
    public class BuildInDependencyInjectionTests
    {
        private static class DataSource
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                builder
                                   .DefineStatefulService(
                                        serviceBuilder =>
                                        {
                                            serviceBuilder
                                               .UseRuntimeRegistrant(Tools.StatefulRuntimeRegistrant)
                                               .DefineDelegate(
                                                    delegateBuilder =>
                                                    {
                                                        delegateBuilder
                                                           .UseDependencies(() => collection)
                                                           .UseDelegate(
                                                                () =>
                                                                {
                                                                });
                                                    });
                                        });
                            }),
                        new[]
                        {
                            typeof(IReliableStateManager),
                            typeof(StatefulServiceContext),
                            typeof(IStatefulServicePartition)
                        }
                    };
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                var wh = new Mock<IWebHostBuilder>();
                                wh.Setup(instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
                                   .Callback<Action<IServiceCollection>>(action => action(collection));

                                builder
                                   .DefineStatefulService(
                                        serviceBuilder =>
                                        {
                                            serviceBuilder
                                               .UseRuntimeRegistrant(Tools.StatefulRuntimeRegistrant)
                                               .DefineAspNetCoreListener(
                                                    listenerBuilder =>
                                                    {
                                                        listenerBuilder
                                                           .UseCommunicationListener(Tools.AspNetCoreCommunicationListenerFunc)
                                                           .UseWebHostBuilder(() => wh.Object);
                                                    });
                                        });
                            }),
                        new[]
                        {
                            typeof(IReliableStateManager),
                            typeof(StatefulServiceContext),
                            typeof(IStatefulServicePartition),
                            typeof(IServiceHostListenerInformation),
                            typeof(IServiceHostAspNetCoreListenerInformation)
                        }
                    };
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                builder
                                   .DefineStatefulService(
                                        serviceBuilder =>
                                        {
                                            serviceBuilder
                                               .UseRuntimeRegistrant(Tools.StatefulRuntimeRegistrant)
                                               .DefineRemotingListener(
                                                    listenerBuilder =>
                                                    {
                                                        listenerBuilder
                                                           .UseCommunicationListener(Tools.RemotingCommunicationListenerFunc)
                                                           .UseImplementation<Tools.TestRemoting>()
                                                           .UseDependencies(() => collection);
                                                    });
                                        });
                            }),
                        new[]
                        {
                            typeof(IReliableStateManager),
                            typeof(StatefulServiceContext),
                            typeof(IStatefulServicePartition),
                            typeof(IServiceHostListenerInformation),
                            typeof(IServiceHostRemotingListenerInformation)
                        }
                    };
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                builder
                                   .DefineStatelessService(
                                        serviceBuilder =>
                                        {
                                            serviceBuilder
                                               .UseRuntimeRegistrant(Tools.StatelessRuntimeRegistrant)
                                               .DefineDelegate(
                                                    delegateBuilder =>
                                                    {
                                                        delegateBuilder
                                                           .UseDependencies(() => collection)
                                                           .UseDelegate(
                                                                () =>
                                                                {
                                                                });
                                                    });
                                        });
                            }),
                        new[]
                        {
                            typeof(StatelessServiceContext),
                            typeof(IStatelessServicePartition)
                        }
                    };
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                var wh = new Mock<IWebHostBuilder>();
                                wh.Setup(instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
                                   .Callback<Action<IServiceCollection>>(action => action(collection));

                                builder
                                   .DefineStatelessService(
                                        serviceBuilder =>
                                        {
                                            serviceBuilder
                                               .UseRuntimeRegistrant(Tools.StatelessRuntimeRegistrant)
                                               .DefineAspNetCoreListener(
                                                    listenerBuilder =>
                                                    {
                                                        listenerBuilder
                                                           .UseCommunicationListener(Tools.AspNetCoreCommunicationListenerFunc)
                                                           .UseWebHostBuilder(() => wh.Object);
                                                    });
                                        });
                            }),
                        new[]
                        {
                            typeof(StatelessServiceContext),
                            typeof(IStatelessServicePartition),
                            typeof(IServiceHostListenerInformation),
                            typeof(IServiceHostAspNetCoreListenerInformation)
                        }
                    };
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                builder
                                   .DefineStatelessService(
                                        serviceBuilder =>
                                        {
                                            serviceBuilder
                                               .UseRuntimeRegistrant(Tools.StatelessRuntimeRegistrant)
                                               .DefineRemotingListener(
                                                    listenerBuilder =>
                                                    {
                                                        listenerBuilder
                                                           .UseCommunicationListener(Tools.RemotingCommunicationListenerFunc)
                                                           .UseImplementation<Tools.TestRemoting>()
                                                           .UseDependencies(() => collection);
                                                    });
                                        });
                            }),
                        new[]
                        {
                            typeof(StatelessServiceContext),
                            typeof(IStatelessServicePartition),
                            typeof(IServiceHostListenerInformation),
                            typeof(IServiceHostRemotingListenerInformation)
                        }
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Data), MemberType = typeof(DataSource))]
        public void
            Should_register_services_When_activating_builder(
                Action<IHostBuilder, IServiceCollection> setupCollection,
                Type[] customTypes)
        {
            // Arrange
            var defaultTypes = new[]
            {
                typeof(ServiceContext),
                typeof(IServicePartition),
                typeof(IServiceEventSource),
                typeof(ILoggerProvider)
            };
            var collection = new Mock<ServiceCollection>
            {
                CallBase = true
            };
            foreach (var type in defaultTypes.Concat(customTypes))
            {
                collection
                   .As<IServiceCollection>()
                   .Setup(instance => instance.Add(It.Is<ServiceDescriptor>(v => v.ServiceType == type)))
                   .Verifiable();
            }

            // Act
            var builder = new HostBuilder();

            setupCollection(builder, collection.Object);

            var host = builder.Build();

            host.StartAsync().GetAwaiter().GetResult();
            host.StopAsync().GetAwaiter().GetResult();

            // Assert
            collection.Verify();
        }
    }
}