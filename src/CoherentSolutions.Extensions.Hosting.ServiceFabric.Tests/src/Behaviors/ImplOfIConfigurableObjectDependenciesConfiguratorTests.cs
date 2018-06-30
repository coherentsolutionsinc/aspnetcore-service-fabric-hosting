using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Xunit;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Behaviors
{
    public class ImplOfIConfigurableObjectDependenciesConfiguratorTests
    {
        public interface IDependencyService : IService
        {
        }

        private static class DataSource
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[]
                    {
                        new StatefulServiceHostBuilder(),
                        new Action<StatefulServiceHostBuilder>(
                            c =>
                            {
                                c.Build();
                            })
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostDelegateReplicaTemplate(),
                        new Action<StatefulServiceHostDelegateReplicaTemplate>(
                            c =>
                            {
                                c.UseDelegate(() => Task.CompletedTask);
                                c.Activate(Tools.StatefulService);
                            })
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostRemotingListenerReplicaTemplate(),
                        new Action<StatefulServiceHostRemotingListenerReplicaTemplate>(
                            c =>
                            {
                                c.UseImplementation(() => new Mock<IDependencyService>().Object);
                                c.Activate(Tools.StatefulService).CreateCommunicationListener(Tools.StatefulContext);
                            })
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostBuilder(),
                        new Action<StatelessServiceHostBuilder>(
                            c =>
                            {
                                c.Build();
                            })
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostDelegateReplicaTemplate(),
                        new Action<StatelessServiceHostDelegateReplicaTemplate>(
                            c =>
                            {
                                c.UseDelegate(() => Task.CompletedTask);
                                c.Activate(Tools.StatelessService);
                            })
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostRemotingListenerReplicaTemplate(),
                        new Action<StatelessServiceHostRemotingListenerReplicaTemplate>(
                            c =>
                            {
                                c.UseImplementation(() => new Mock<IDependencyService>().Object);
                                c.Activate(Tools.StatelessService).CreateCommunicationListener(Tools.StatelessContext);
                            })
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Data), MemberType = typeof(DataSource))]
        public void Should_use_collection_from_UseDependencies_When_configuring_dependencies_by_ConfigureDependencies<TBuilder>(
            TBuilder configurableObject,
            Action<TBuilder> invoke)
            where TBuilder : IConfigurableObject<IConfigurableObjectDependenciesConfigurator>
        {
            // Arrange
            var descriptor = new ServiceDescriptor(typeof(ImplOfIConfigurableObjectDependenciesConfiguratorTests), this);

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