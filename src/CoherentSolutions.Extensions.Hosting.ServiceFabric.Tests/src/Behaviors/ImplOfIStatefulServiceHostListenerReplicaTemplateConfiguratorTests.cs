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
    public class ImplOfIStatefulServiceHostListenerReplicaTemplateConfiguratorTests
    {
        private static class DataSource
        {
            public interface IDependencyService : IService
            {
            }

            public static IEnumerable<object[]> Data
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
        }

        [Theory]
        [MemberData(nameof(DataSource.Data), MemberType = typeof(DataSource))]
        public void
            Should_configure_replica_listener_to_listen_on_secondary_When_configured_by_UseListenerOnSecondary<T>(
                T instance,
                Action<T> configure,
                Func<T, ServiceReplicaListener> factory)
            where T : IConfigurableObject<IStatefulServiceHostListenerReplicaTemplateConfigurator>
        {
            // Arrange
            configure(instance);

            instance.ConfigureObject(
                c =>
                {
                    c.UseListenerOnSecondary();
                });

            // Act
            var listener = factory(instance);

            // Assert
            Assert.True(listener.ListenOnSecondary);
        }
    }
}