using System;
using System.Collections.Generic;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features.DefinesAndBuild
{
    public class DefineAndBuildAspNetCoreListenersTests
    {
        private static class DataSource
        {
            public static IEnumerable<object[]> Listeners
            {
                get
                {
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>>
                            >(
                                (
                                    builder,
                                    action) =>
                                {
                                    builder
                                       .DefineStatefulService(
                                            serviceBuilder =>
                                            {
                                                serviceBuilder
                                                   .UseRuntimeRegistrant(Tools.StatefulRuntimeRegistrant)
                                                   .DefineAspNetCoreListener(action);
                                            });
                                })
                           .WithDescription("StatefulService")
                    };
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>>
                            >(
                                (
                                    builder,
                                    action) =>
                                {
                                    builder
                                       .DefineStatelessService(
                                            serviceBuilder =>
                                            {
                                                serviceBuilder
                                                   .UseRuntimeRegistrant(Tools.StatelessRuntimeRegistrant)
                                                   .DefineAspNetCoreListener(action);
                                            });
                                })
                           .WithDescription("StatelessService")
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Listeners), MemberType = typeof(DataSource))]
        public void DefineAndBuildAspNetCoreListener(
            WithDescription<Action<HostBuilder, Action<IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>>
            >> config)
        {
            // Arrange
            var mockWebHostBuilder = new Mock<IWebHostBuilder>();
            mockWebHostBuilder
               .Setup(instance => instance.Build())
               .Verifiable();

            var arrangeEndpoint = "Endpoint";
            var arrangeWebHostBuilder = mockWebHostBuilder.Object;

            object expectedEndpoint = arrangeEndpoint;
            object actualEndpoint = null;

            object expectedWebHostBuilder = arrangeWebHostBuilder;
            object actualWebHostBuilder = null;

            var builder = new HostBuilder();

            // Act
            config.Target(
                builder,
                template =>
                {
                    template.ConfigureObject(
                        c =>
                        {
                            c.UseCommunicationListener(
                                (
                                    context,
                                    endpointName,
                                    build) =>
                                {
                                    actualEndpoint = endpointName;

                                    return Tools.AspNetCoreCommunicationListenerFunc(context, endpointName, build);
                                });
                            c.UseEndpoint(arrangeEndpoint);
                            c.UseWebHostBuilder(() => arrangeWebHostBuilder);
                            c.ConfigureWebHost(
                                webHostBuilder =>
                                {
                                    actualWebHostBuilder = webHostBuilder;
                                });
                        });
                });

            var host = builder.Build();

            host.StartAsync().GetAwaiter().GetResult();
            host.StopAsync().GetAwaiter().GetResult();

            // Assert
            Assert.Same(expectedEndpoint, actualEndpoint);
            Assert.Same(expectedWebHostBuilder, actualWebHostBuilder);

            mockWebHostBuilder.Verify();
        }
    }
}