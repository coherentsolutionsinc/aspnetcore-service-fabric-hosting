﻿using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Remoting;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features
{
    public class HierarchyDependencyInjectionTests
    {
        public interface IDependency : IService
        {
        }

        public class Dependency : IDependency
        {
        }

        private static class DataSource
        {
            private interface IMyRemotingImplementation : IService
            {
            }

            private class MyRemotingImplementation : IMyRemotingImplementation
            {
            }

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
                                builder.DefineStatefulService(
                                    serviceBuilder =>
                                    {
                                        serviceBuilder
                                           .UseRuntimeRegistrant(Tools.StatefulRuntimeRegistrant)
                                           .UseDependencies(() => collection);
                                    });
                            })
                    };
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                builder.DefineStatefulService(
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
                            })
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

                                builder.DefineStatefulService(
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
                            })
                    };
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                builder.DefineStatefulService(
                                    serviceBuilder =>
                                    {
                                        serviceBuilder
                                           .UseRuntimeRegistrant(Tools.StatefulRuntimeRegistrant)
                                           .DefineRemotingListener(
                                                listenerBuilder =>
                                                {
                                                    listenerBuilder
                                                       .UseCommunicationListener(Tools.RemotingCommunicationListenerFunc)
                                                       .UseImplementation<Dependency>()
                                                       .UseDependencies(() => collection);
                                                });
                                    });
                            })
                    };
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                builder.DefineStatelessService(
                                    serviceBuilder =>
                                    {
                                        serviceBuilder
                                           .UseRuntimeRegistrant(Tools.StatelessRuntimeRegistrant)
                                           .UseDependencies(() => collection);
                                    });
                            })
                    };
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                builder.DefineStatelessService(
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
                            })
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

                                builder.DefineStatelessService(
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
                            })
                    };
                    yield return new object[]
                    {
                        new Action<IHostBuilder, IServiceCollection>(
                            (
                                builder,
                                collection) =>
                            {
                                builder.DefineStatelessService(
                                    serviceBuilder =>
                                    {
                                        serviceBuilder
                                           .UseRuntimeRegistrant(Tools.StatelessRuntimeRegistrant)
                                           .DefineRemotingListener(
                                                listenerBuilder =>
                                                {
                                                    listenerBuilder
                                                       .UseCommunicationListener(Tools.RemotingCommunicationListenerFunc)
                                                       .UseImplementation<Dependency>()
                                                       .UseDependencies(() => collection);
                                                });
                                    });
                            })
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Data), MemberType = typeof(DataSource))]
        public void
            Should_reregister_services_on_next_level_When_configuring_services_upper_level(
                Action<IHostBuilder, IServiceCollection> setupCollection)
        {
            // Arrange
            var singleton = new ServiceDescriptor(typeof(HierarchyDependencyInjectionTests), this);
            var transient = new ServiceDescriptor(typeof(IDependency), typeof(Dependency), ServiceLifetime.Transient);

            var collection = new Mock<ServiceCollection>
            {
                CallBase = true
            };
            collection
               .As<IServiceCollection>()
               .Setup(instance => instance.Add(It.Is<ServiceDescriptor>(v => v.ImplementationInstance == this)))
               .Verifiable();

            collection
               .As<IServiceCollection>()
               .Setup(
                    instance =>
                        instance
                           .Add(It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IDependency) && v.ImplementationType == typeof(Dependency))))
               .Verifiable();

            // Act
            var builder = new HostBuilder()
               .ConfigureServices(
                    services =>
                    {
                        services.Add(singleton);
                        services.Add(transient);
                    });

            setupCollection(builder, collection.Object);

            var host = builder.Build();

            host.StartAsync().GetAwaiter().GetResult();
            host.StopAsync().GetAwaiter().GetResult();

            // Assert
            collection.Verify();
        }
    }
}