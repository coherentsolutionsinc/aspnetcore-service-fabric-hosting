using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Contracts
{
    public class DependencyInjectionContractTests
    {
        private static class DataSource
        {
            private interface IMyRemotingImplementation : Microsoft.ServiceFabric.Services.Remoting.IService
            {
            }

            private class MyRemotingImplementation : IMyRemotingImplementation
            { }


            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[]
                    {
                        new StatefulServiceHostDelegateReplicaTemplate(),
                        new Action<StatefulServiceHostDelegateReplicaTemplate, IServiceCollection>(
                            (
                                c,
                                services) =>
                            {
                                c.UseDependencies(() => services);
                                c.UseDelegate(
                                    () =>
                                    {
                                    });
                            }),
                        new Action<StatefulServiceHostDelegateReplicaTemplate>(
                            c =>
                            {
                                c.Activate(Tools.StatefulService);
                            }),
                        new[]
                        {
                            typeof(IReliableStateManager)
                        }
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostAspNetCoreListenerReplicaTemplate(),
                        new Action<StatefulServiceHostAspNetCoreListenerReplicaTemplate, IServiceCollection>(
                            (
                                c,
                                services) =>
                            {
                                var builder = new Mock<IWebHostBuilder>();
                                builder
                                   .Setup(instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
                                   .Callback<Action<IServiceCollection>>(action => action(services));

                                c.UseWebHostBuilder(() => builder.Object);
                                c.UseCommunicationListener(Tools.StatefulAspNetCoreCommunicationListenerFunc);
                            }),
                        new Action<StatefulServiceHostAspNetCoreListenerReplicaTemplate>(
                            c =>
                            {
                                c.Activate(Tools.StatefulService)
                                   .CreateCommunicationListener(Tools.StatefulContext)
                                   .OpenAsync(CancellationToken.None);
                            }),
                        new[]
                        {
                            typeof(IReliableStateManager)
                        }
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostRemotingListenerReplicaTemplate(),
                        new Action<StatefulServiceHostRemotingListenerReplicaTemplate, IServiceCollection>(
                            (
                                c,
                                services) =>
                            {
                                c.UseDependencies(() => services);
                                c.UseImplementation<MyRemotingImplementation>();
                            }),
                        new Action<StatefulServiceHostRemotingListenerReplicaTemplate>(
                            c =>
                            {
                                c.Activate(Tools.StatefulService)
                                   .CreateCommunicationListener(Tools.StatefulContext)
                                   .OpenAsync(CancellationToken.None);
                            }),
                        new[]
                        {
                            typeof(IReliableStateManager)
                        }
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostDelegateReplicaTemplate(),
                        new Action<StatelessServiceHostDelegateReplicaTemplate, IServiceCollection>(
                            (
                                c,
                                services) =>
                            {
                                c.UseDependencies(() => services);
                                c.UseDelegate(
                                    () =>
                                    {
                                    });
                            }),
                        new Action<StatelessServiceHostDelegateReplicaTemplate>(
                            c =>
                            {
                                c.Activate(Tools.StatelessService);
                            }),
                        Type.EmptyTypes
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostAspNetCoreListenerReplicaTemplate(),
                        new Action<StatelessServiceHostAspNetCoreListenerReplicaTemplate, IServiceCollection>(
                            (
                                c,
                                services) =>
                            {
                                var builder = new Mock<IWebHostBuilder>();
                                builder
                                   .Setup(instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
                                   .Callback<Action<IServiceCollection>>(action => action(services));

                                c.UseWebHostBuilder(() => builder.Object);
                                c.UseCommunicationListener(Tools.StatefulAspNetCoreCommunicationListenerFunc);
                            }),
                        new Action<StatelessServiceHostAspNetCoreListenerReplicaTemplate>(
                            c =>
                            {
                                c.Activate(Tools.StatelessService)
                                   .CreateCommunicationListener(Tools.StatelessContext)
                                   .OpenAsync(CancellationToken.None);
                            }),
                        Type.EmptyTypes
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostRemotingListenerReplicaTemplate(),
                        new Action<StatelessServiceHostRemotingListenerReplicaTemplate, IServiceCollection>(
                            (
                                c,
                                services) =>
                            {
                                c.UseDependencies(() => services);
                                c.UseImplementation<MyRemotingImplementation>();
                            }),
                        new Action<StatelessServiceHostRemotingListenerReplicaTemplate>(
                            c =>
                            {
                                c.Activate(Tools.StatelessService)
                                   .CreateCommunicationListener(Tools.StatelessContext)
                                   .OpenAsync(CancellationToken.None);
                            }),
                        Type.EmptyTypes
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Data), MemberType = typeof(DataSource))]
        public void
            Should_configure_services_When_activating_delegate_replica_template<TBuilder>(
                TBuilder builder,
                Action<TBuilder, IServiceCollection> configure,
                Action<TBuilder> build,
                Type[] customTypes)
        {
            // Arrange
            var defaultTypes = new[]
            {
                typeof(ServiceContext),
                typeof(IServicePartition),
                typeof(IServiceEventSource)
            };
            var services = new Mock<ServiceCollection>
            {
                CallBase = true
            };
            services
               .As<IServiceCollection>()
               .Setup(instance => instance.Add(It.IsAny<ServiceDescriptor>()));

            // Act
            configure(builder, services.Object);
            build(builder);

            // Assert
            foreach (var type in customTypes.Concat(defaultTypes))
            {
                services
                   .As<IServiceCollection>()
                   .Verify(instance => instance.Add(It.Is<ServiceDescriptor>(v => type == v.ServiceType)), Times.Once());
            }
        }
    }
}