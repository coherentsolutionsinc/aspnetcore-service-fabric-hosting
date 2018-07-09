using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features
{
    public class ConfigureActionTests
    {
        private static class DataSource
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IConfigurableObject<object>>
                            >(
                                (
                                    builder,
                                    action) =>
                                {
                                    builder.DefineStatefulService(
                                        serviceBuilder =>
                                        {
                                            serviceBuilder
                                               .UseRuntimeRegistrant(Tools.StatefulRuntimeRegistrant);

                                            action(serviceBuilder);
                                        });
                                })
                           .WithDescription("StatefulService")
                    };
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IConfigurableObject<object>>
                            >(
                                (
                                    builder,
                                    action) =>
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
                                                           .UseDelegate(() => Task.CompletedTask);

                                                        action(delegateBuilder);
                                                    });
                                        });
                                })
                           .WithDescription("StatefulService-Delegate")
                    };
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IConfigurableObject<object>>
                            >(
                                (
                                    builder,
                                    action) =>
                                {
                                    builder.DefineStatefulService(
                                        serviceBuilder =>
                                        {
                                            serviceBuilder
                                               .UseRuntimeRegistrant(Tools.StatefulRuntimeRegistrant)
                                               .DefineAspNetCoreListener(
                                                    listenerBuilder =>
                                                    {
                                                        listenerBuilder
                                                           .UseWebHostBuilder(() => new Mock<IWebHostBuilder>().Object)
                                                           .UseCommunicationListener(Tools.AspNetCoreCommunicationListenerFunc);

                                                        action(listenerBuilder);
                                                    });
                                        });
                                })
                           .WithDescription("StatefulService-AspNetCoreListener")
                    };
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IConfigurableObject<object>>
                            >(
                                (
                                    builder,
                                    action) =>
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
                                                           .UseImplementation<Tools.TestRemoting>();

                                                        action(listenerBuilder);
                                                    });
                                        });
                                })
                           .WithDescription("StatefulService-RemotingListener")
                    };
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IConfigurableObject<object>>
                            >(
                                (
                                    builder,
                                    action) =>
                                {
                                    builder.DefineStatelessService(
                                        serviceBuilder =>
                                        {
                                            serviceBuilder
                                               .UseRuntimeRegistrant(Tools.StatelessRuntimeRegistrant);

                                            action(serviceBuilder);
                                        });
                                })
                           .WithDescription("StatelessService")
                    };
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IConfigurableObject<object>>
                            >(
                                (
                                    builder,
                                    action) =>
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
                                                           .UseDelegate(() => Task.CompletedTask);

                                                        action(delegateBuilder);
                                                    });
                                        });
                                })
                           .WithDescription("StatelessService-Delegate")
                    };
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IConfigurableObject<object>>
                            >(
                                (
                                    builder,
                                    action) =>
                                {
                                    builder.DefineStatelessService(
                                        serviceBuilder =>
                                        {
                                            serviceBuilder
                                               .UseRuntimeRegistrant(Tools.StatelessRuntimeRegistrant)
                                               .DefineAspNetCoreListener(
                                                    listenerBuilder =>
                                                    {
                                                        listenerBuilder
                                                           .UseWebHostBuilder(() => new Mock<IWebHostBuilder>().Object)
                                                           .UseCommunicationListener(Tools.AspNetCoreCommunicationListenerFunc);

                                                        action(listenerBuilder);
                                                    });
                                        });
                                })
                           .WithDescription("StatelessService-AspNetCoreListener")
                    };
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IConfigurableObject<object>>
                            >(
                                (
                                    builder,
                                    action) =>
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
                                                           .UseImplementation<Tools.TestRemoting>();

                                                        action(listenerBuilder);
                                                    });
                                        });
                                })
                           .WithDescription("StatelessService-RemotingListener")
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Data), MemberType = typeof(DataSource))]
        public void
            ConfigureObjectActionCalled(
                WithDescription<Action<HostBuilder, Action<IConfigurableObject<object>>
                >> config)
        {
            // Arrange
            var mockDelegate = new Mock<Action<object>>();
            mockDelegate
               .Setup(instance => instance(It.IsAny<object>()))
               .Verifiable();

            var arrangeDelegate = mockDelegate.Object;

            var builder = new HostBuilder();

            // Act
            config.Target(
                builder,
                template =>
                {
                    template.ConfigureObject(arrangeDelegate);
                });

            var host = builder.Build();

            host.StartAsync().GetAwaiter().GetResult();
            host.StopAsync().GetAwaiter().GetResult();

            // Assert
            mockDelegate.Verify();
        }
    }
}