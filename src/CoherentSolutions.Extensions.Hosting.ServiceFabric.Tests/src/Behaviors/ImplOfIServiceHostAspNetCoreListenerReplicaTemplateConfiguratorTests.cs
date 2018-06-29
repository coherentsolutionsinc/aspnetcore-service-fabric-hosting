using System;
using System.Collections.Generic;
using System.Threading;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Behaviors
{
    public class ImplOfIServiceHostAspNetCoreListenerReplicaTemplateConfiguratorTests
    {
        private static class DataSource
        {
            public static IEnumerable<object[]> UseCommunicationListenersData
            {
                get
                {
                    yield return new object[]
                    {
                        new StatefulServiceHostAspNetCoreListenerReplicaTemplate(),
                        Tools.DefaultStatefulAspNetCoreCommunicationListener,
                        new Func<StatefulServiceHostAspNetCoreListenerReplicaTemplate, ICommunicationListener>(
                            c =>
                            {
                                return c.Activate(Tools.StatefulService).CreateCommunicationListener(Tools.StatefulContext);
                            })
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostAspNetCoreListenerReplicaTemplate(),
                        Tools.DefaultStatelessAspNetCoreCommunicationListener,
                        new Func<StatelessServiceHostAspNetCoreListenerReplicaTemplate, ICommunicationListener>(
                            c =>
                            {
                                return c.Activate(Tools.StatelessService).CreateCommunicationListener(Tools.StatelessContext);
                            })
                    };
                }
            }

            public static IEnumerable<object[]> UseWebHostBuildersData
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
                                c.Activate(Tools.StatefulService)
                                   .CreateCommunicationListener(Tools.StatefulContext)
                                   .OpenAsync(CancellationToken.None);
                            })
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostAspNetCoreListenerReplicaTemplate(),
                        new Action<StatelessServiceHostAspNetCoreListenerReplicaTemplate>(
                            c =>
                            {
                                c.UseCommunicationListener(Tools.AspNetCoreCommunicationListenerFunc);
                                c.Activate(Tools.StatelessService)
                                   .CreateCommunicationListener(Tools.StatelessContext)
                                   .OpenAsync(CancellationToken.None);
                            })
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.UseCommunicationListenersData), MemberType = typeof(DataSource))]
        public void Should_use_communication_listener_from_UseCommunicationListener_When_activating_template<TBuilder>(
            TBuilder configurableObject,
            AspNetCoreCommunicationListener listener,
            Func<TBuilder, ICommunicationListener> invoke)
            where TBuilder : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            // Arrange
            var root = listener;

            object expectedListener = root;
            object actualListener = null;

            // Act
            configurableObject.ConfigureObject(
                config =>
                {
                    config.UseCommunicationListener(
                        (
                            context,
                            s,
                            arg3) => root);
                });

            actualListener = invoke(configurableObject);

            // Assert
            Assert.Same(expectedListener, actualListener);
        }

        [Theory]
        [MemberData(nameof(DataSource.UseWebHostBuildersData), MemberType = typeof(DataSource))]
        public void Should_use_webhost_builder_from_UseWebHostBuilder_When_configuring_webhost_in_ConfigureWebHost<TBuilder>(
            TBuilder configurableObject,
            Action<TBuilder> invoke)
            where TBuilder : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            // Arrange
            var root = new Mock<IWebHostBuilder>();

            object expectedBuilder = root.Object;
            object actualBuilder = null;

            // Act
            configurableObject.ConfigureObject(
                config =>
                {
                    config.UseWebHostBuilder(() => root.Object);
                    config.ConfigureWebHost(
                        builder =>
                        {
                            actualBuilder = builder;
                        });
                });

            invoke(configurableObject);

            // Assert
            Assert.Same(expectedBuilder, actualBuilder);
        }
    }
}