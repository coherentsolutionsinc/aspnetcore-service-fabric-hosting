using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;

using Moq;

using ServiceFabric.Mocks;

using Xunit;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Configurators
{
    public class ConfigurableObjectDependenciesConfiguratorTests
    {
        public interface IDependencyService : IService
        {
        }

        private static class ConfigurableObjectDependenciesConfiguratorDataSource
        {
            private class EndpointResourceDescriptionCollection : KeyedCollection<string, EndpointResourceDescription>
            {
                protected override string GetKeyForItem(
                    EndpointResourceDescription item)
                {
                    return item.Name;
                }
            }

            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[]
                    {
                        new StatefulServiceHostBuilder(),
                        new Action<IConfigurableObject<IConfigurableObjectDependenciesConfigurator>>(
                            c =>
                            {
                                ((StatefulServiceHostBuilder) c).Build();
                            })
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostDelegateReplicaTemplate()
                           .UseDelegate(() => Task.CompletedTask),
                        new Action<IConfigurableObject<IConfigurableObjectDependenciesConfigurator>>(
                            c =>
                            {
                                ((StatefulServiceHostDelegateReplicaTemplate) c).Activate(StatefulService);
                            })
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostRemotingListenerReplicaTemplate()
                           .UseImplementation(() => new Mock<IDependencyService>().Object),
                        new Action<IConfigurableObject<IConfigurableObjectDependenciesConfigurator>>(
                            c =>
                            {
                                ((StatefulServiceHostRemotingListenerReplicaTemplate) c)
                                   .Activate(StatefulService)
                                   .CreateCommunicationListener(StatefulContext);
                            })
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostBuilder(),
                        new Action<IConfigurableObject<IConfigurableObjectDependenciesConfigurator>>(
                            c =>
                            {
                                ((StatelessServiceHostBuilder) c).Build();
                            })
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostDelegateReplicaTemplate()
                           .UseDelegate(() => Task.CompletedTask),
                        new Action<IConfigurableObject<IConfigurableObjectDependenciesConfigurator>>(
                            c =>
                            {
                                ((StatelessServiceHostDelegateReplicaTemplate) c).Activate(StatelessService);
                            })
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostRemotingListenerReplicaTemplate()
                           .UseImplementation(() => new Mock<IDependencyService>().Object),
                        new Action<IConfigurableObject<IConfigurableObjectDependenciesConfigurator>>(
                            c =>
                            {
                                ((StatelessServiceHostRemotingListenerReplicaTemplate) c)
                                   .Activate(StatelessService)
                                   .CreateCommunicationListener(StatelessContext);
                            })
                    };
                }
            }

            private static StatefulServiceContext StatefulContext
            {
                get
                {
                    var package = new Mock<ICodePackageActivationContext>();
                    package
                       .Setup(instance => instance.GetEndpoints())
                       .Returns(
                            new EndpointResourceDescriptionCollection
                            {
                                new EndpointResourceDescription
                                {
                                    Name = "ServiceEndpoint"
                                }
                            });

                    return MockStatefulServiceContextFactory.Create(
                        package.Object,
                        MockStatefulServiceContextFactory.ServiceTypeName,
                        new Uri(MockStatefulServiceContextFactory.ServiceName),
                        Guid.Empty,
                        default(long));
                }
            }

            private static StatelessServiceContext StatelessContext
            {
                get
                {
                    var package = new Mock<ICodePackageActivationContext>();
                    package
                       .Setup(instance => instance.GetEndpoints())
                       .Returns(
                            new EndpointResourceDescriptionCollection
                            {
                                new EndpointResourceDescription
                                {
                                    Name = "ServiceEndpoint"
                                }
                            });

                    return MockStatelessServiceContextFactory.Create(
                        package.Object,
                        MockStatelessServiceContextFactory.ServiceTypeName,
                        new Uri(MockStatelessServiceContextFactory.ServiceName),
                        Guid.Empty,
                        default(long));
                }
            }

            private static IStatefulService StatefulService
            {
                get
                {
                    var service = new Mock<IStatefulService>();
                    service.Setup(instance => instance.GetContext()).Returns(MockStatefulServiceContextFactory.Default);
                    service.Setup(instance => instance.GetPartition()).Returns(new Mock<IStatefulServicePartition>().Object);
                    service.Setup(instance => instance.GetEventSource()).Returns(new Mock<IServiceEventSource>().Object);
                    service.Setup(instance => instance.GetReliableStateManager()).Returns(new Mock<IReliableStateManager>().Object);
                    return service.Object;
                }
            }

            private static IStatelessService StatelessService
            {
                get
                {
                    var service = new Mock<IStatelessService>();
                    service.Setup(instance => instance.GetContext()).Returns(MockStatelessServiceContextFactory.Default);
                    service.Setup(instance => instance.GetPartition()).Returns(new Mock<IStatelessServicePartition>().Object);
                    service.Setup(instance => instance.GetEventSource()).Returns(new Mock<IServiceEventSource>().Object);
                    return service.Object;
                }
            }
        }

        [Theory]
        [MemberData(nameof(ConfigurableObjectDependenciesConfiguratorDataSource.Data), MemberType = typeof(ConfigurableObjectDependenciesConfiguratorDataSource))]
        public void Should_use_collection_provided_by_UseDependencies_When_configuring_dependencies_by_ConfigureDependencies(
            IConfigurableObject<IConfigurableObjectDependenciesConfigurator> configurableObject,
            Action<IConfigurableObject<IConfigurableObjectDependenciesConfigurator>> invoke)
        {
            // Arrange
            var descriptor = new ServiceDescriptor(typeof(ConfigurableObjectDependenciesConfiguratorTests), this);

            var collection = new Mock<ServiceCollection>
            {
                CallBase = true
            };
            collection
               .As<IServiceCollection>()
               .Setup(instance => instance.Add(descriptor))
               .Verifiable();

            // Act
            configurableObject.ConfigureObject(
                config =>
                {
                    config.UseDependencies(() => collection.Object);
                    config.ConfigureDependencies(
                        services =>
                        {
                            services.Add(descriptor);
                        });
                });

            invoke(configurableObject);

            // Assert
            collection.Verify();
        }
    }
}