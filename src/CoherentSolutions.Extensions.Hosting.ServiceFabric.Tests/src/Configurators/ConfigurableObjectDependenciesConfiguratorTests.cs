using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;

using Moq;

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
                                ((StatefulServiceHostDelegateReplicaTemplate) c).Activate(Tools.StatefulService);
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
                                   .Activate(Tools.StatefulService)
                                   .CreateCommunicationListener(Tools.StatefulContext);
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
                                ((StatelessServiceHostDelegateReplicaTemplate) c).Activate(Tools.StatelessService);
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
                                   .Activate(Tools.StatelessService)
                                   .CreateCommunicationListener(Tools.StatelessContext);
                            })
                    };
                }
            }
        }

        [Theory]
        [MemberData(
            nameof(ConfigurableObjectDependenciesConfiguratorDataSource.Data),
            MemberType = typeof(ConfigurableObjectDependenciesConfiguratorDataSource))]
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