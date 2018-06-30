using System;
using System.Collections.Generic;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

using Moq;

using Xunit;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Behaviors
{
    public class ImplOfIServiceHostListenerReplicaTemplateConfiguratorTests
    {
        private static class DataSource
        {
            public interface IDependencyService : IService
            {
            }

            public static IEnumerable<object[]> StatefulData
            {
                get
                {
                    yield return new object[]
                    {
                        new StatefulServiceHostAspNetCoreListenerReplicaTemplate(),
                        new Action<StatefulServiceHostAspNetCoreListenerReplicaTemplate>(
                            c =>
                            {
                                c.UseCommunicationListener(Tools.AspNetCoreCommunicationListenerFunc);
                                c.UseWebHostBuilder(() => new Mock<IWebHostBuilder>().Object);
                            }),
                        new Func<StatefulServiceHostAspNetCoreListenerReplicaTemplate, ServiceReplicaListener>(
                            c =>
                            {
                                return c.Activate(Tools.StatefulService);
                            })
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostRemotingListenerReplicaTemplate(),
                        new Action<StatefulServiceHostRemotingListenerReplicaTemplate>(
                            c =>
                            {
                                c.UseImplementation(() => new Mock<IDependencyService>().Object);
                            }),
                        new Func<StatefulServiceHostRemotingListenerReplicaTemplate, ServiceReplicaListener>(
                            c =>
                            {
                                return c.Activate(Tools.StatefulService);
                            })
                    };
                }
            }

            public static IEnumerable<object[]> StatelessData
            {
                get
                {
                    yield return new object[]
                    {
                        new StatelessServiceHostAspNetCoreListenerReplicaTemplate(),
                        new Action<StatelessServiceHostAspNetCoreListenerReplicaTemplate>(
                            c =>
                            {
                                c.UseCommunicationListener(Tools.AspNetCoreCommunicationListenerFunc);
                                c.UseWebHostBuilder(() => new Mock<IWebHostBuilder>().Object);
                            }),
                        new Func<StatelessServiceHostAspNetCoreListenerReplicaTemplate, ServiceInstanceListener>(
                            c =>
                            {
                                return c.Activate(Tools.StatelessService);
                            })
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostRemotingListenerReplicaTemplate(),
                        new Action<StatelessServiceHostRemotingListenerReplicaTemplate>(
                            c =>
                            {
                                c.UseImplementation(() => new Mock<IDependencyService>().Object);
                            }),
                        new Func<StatelessServiceHostRemotingListenerReplicaTemplate, ServiceInstanceListener>(
                            c =>
                            {
                                return c.Activate(Tools.StatelessService);
                            })
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.StatefulData), MemberType = typeof(DataSource))]
        public void
            Should_configure_replica_listener_name_When_configured_by_UseEndpointName<T>(
                T instance,
                Action<T> configure,
                Func<T, ServiceReplicaListener> factory)
            where T : IConfigurableObject<IServiceHostListenerReplicaTemplateConfigurator>
        {
            // Arrange
            configure(instance);

            instance.ConfigureObject(
                c =>
                {
                    c.UseEndpoint("ServiceEndpoint");
                });

            // Act
            var listener = factory(instance);

            // Assert
            Assert.Equal("ServiceEndpoint", listener.Name);
        }

        [Theory]
        [MemberData(nameof(DataSource.StatelessData), MemberType = typeof(DataSource))]
        public void
            Should_configure_instance_listener_name_When_configured_by_UseEndpointName<T>(
                T instance,
                Action<T> configure,
                Func<T, ServiceInstanceListener> factory)
            where T : IConfigurableObject<IServiceHostListenerReplicaTemplateConfigurator>
        {
            // Arrange
            configure(instance);

            instance.ConfigureObject(
                c =>
                {
                    c.UseEndpoint("ServiceEndpoint");
                });

            // Act
            var listener = factory(instance);

            // Assert
            Assert.Equal("ServiceEndpoint", listener.Name);
        }
    }
}