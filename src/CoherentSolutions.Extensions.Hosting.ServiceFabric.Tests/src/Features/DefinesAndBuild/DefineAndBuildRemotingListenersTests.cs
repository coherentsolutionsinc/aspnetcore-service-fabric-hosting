using System;
using System.Collections.Generic;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features.DefinesAndBuild
{
    public class DefineAndBuildRemotingListenersTests
    {
        private static class DataSource
        {
            public static IEnumerable<object[]> Listeners
            {
                get
                {
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>>>(
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
                                                   .DefineRemotingListener(action);
                                            });
                                })
                           .WithDescription("StatefulService")
                    };
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>>>(
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
                                                   .DefineRemotingListener(action);
                                            });
                                })
                           .WithDescription("StatelessService")
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Listeners), MemberType = typeof(DataSource))]
        public void DefineAndBuildRemotingListener(
            WithDescription<Action<HostBuilder, Action<IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>>>>
                config)
        {
            // Arrange
            var mockImplementation = new Mock<Tools.ITestRemoting>();
            var mockSerializer = new Mock<IServiceRemotingMessageSerializationProvider>();

            var arrangeEndpoint = "Endpoint";
            var arrangeImplementation = mockImplementation.Object;
            var arrangeSerializer = mockSerializer.Object;
            var arrangeSettings = new FabricTransportRemotingListenerSettings();

            object expectedEndpoint = arrangeEndpoint;
            object actualEndpoint = null;

            object expectedSerializer = arrangeSerializer;
            object actualSerializer = null;

            object expectedSettings = arrangeSettings;
            object actualSettings = null;

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
                                    build) =>
                                {
                                    var options = build(context);

                                    actualEndpoint = options.ListenerSettings.EndpointResourceName;
                                    actualSerializer = options.MessageSerializationProvider;
                                    actualSettings = options.ListenerSettings;

                                    return Tools.RemotingCommunicationListenerFunc(context, build);
                                });
                            c.UseEndpoint(arrangeEndpoint);
                            c.UseSerializer(provider => arrangeSerializer);
                            c.UseImplementation(provider => arrangeImplementation);
                            c.UseSettings(() => arrangeSettings);
                        });
                });

            var host = builder.Build();

            host.StartAsync().GetAwaiter().GetResult();
            host.StopAsync().GetAwaiter().GetResult();

            // Assert
            Assert.Same(expectedEndpoint, actualEndpoint);
            Assert.Same(expectedSerializer, actualSerializer);
            Assert.Same(expectedSettings, actualSettings);
        }
    }
}