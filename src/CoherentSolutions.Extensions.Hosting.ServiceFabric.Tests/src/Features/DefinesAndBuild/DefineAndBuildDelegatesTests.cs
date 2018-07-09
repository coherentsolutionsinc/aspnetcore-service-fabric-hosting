using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.Extensions.Hosting;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features.DefinesAndBuild
{
    public class DefineAndBuildDelegatesTests
    {
        private static class DataSource
        {
            public static IEnumerable<object[]> Delegates
            {
                get
                {
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>>
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
                                                   .DefineDelegate(action);
                                            });
                                })
                           .WithDescription("StatefulService")
                    };
                    yield return new object[]
                    {
                        new Action<HostBuilder, Action<IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>>
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
                                                   .DefineDelegate(action);
                                            });
                                })
                           .WithDescription("StatelessService")
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Delegates), MemberType = typeof(DataSource))]
        public void DefineAndBuildDelegate(
            WithDescription<Action<HostBuilder, Action<IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>>
            >> config)
        {
            // Arrange
            var mockDelegate = new Mock<Action>();

            var mockDelegateInvoker = new Mock<IServiceHostDelegateInvoker>();
            mockDelegateInvoker
               .Setup(instance => instance.InvokeAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask)
               .Verifiable();

            var arrangeDelegate = mockDelegate.Object;
            var arrangeDelegateInvoker = mockDelegateInvoker.Object;

            object expectedDelegate = arrangeDelegate;
            object actualDelegate = null;

            var builder = new HostBuilder();

            // Act
            config.Target(
                builder,
                template =>
                {
                    template.ConfigureObject(
                        c =>
                        {
                            c.UseDelegateInvoker(
                                (
                                    @delegate,
                                    provider) =>
                                {
                                    actualDelegate = @delegate;

                                    return arrangeDelegateInvoker;
                                });
                            c.UseDelegate(arrangeDelegate);
                        });
                });

            var host = builder.Build();

            host.StartAsync().GetAwaiter().GetResult();
            host.StopAsync().GetAwaiter().GetResult();

            // Assert
            Assert.Same(expectedDelegate, actualDelegate);

            mockDelegateInvoker.Verify();
        }
    }
}