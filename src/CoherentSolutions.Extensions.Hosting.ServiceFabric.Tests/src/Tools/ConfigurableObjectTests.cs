using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Tools
{
    public class ConfigurableObjectTests
    {
        private static class DataSource
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[]
                    {
                        new StatefulServiceHostBuilder(),
                        new Mock<IStatefulServiceHostBuilderConfigurator>().Object
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostDelegateReplicaTemplate()
                           .UseLifecycleEvent(ServiceLifecycleEvent.OnRunAsyncWhenAllListenersOpened)
                           .UseDelegate(() => Task.CompletedTask),
                        new Mock<IStatefulServiceHostDelegateReplicaTemplateConfigurator>().Object
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostAspNetCoreListenerReplicaTemplate()
                           .UseCommunicationListener((
                                context,
                                s,
                                arg3) => new Mock<AspNetCoreCommunicationListener>().Object),
                        new Mock<IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator>().Object
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostRemotingListenerReplicaTemplate()
                           .UseImplementation(() => new Mock<ConfigurableObjectDependenciesConfiguratorTests.IDependencyService>().Object),
                        new Mock<IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator>().Object
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostBuilder(),
                        new Mock<IStatelessServiceHostBuilderConfigurator>().Object
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostDelegateReplicaTemplate()
                           .UseLifecycleEvent(ServiceLifecycleEvent.OnRunAsyncWhenAllListenersOpened)
                           .UseDelegate(() => Task.CompletedTask),
                        new Mock<IStatelessServiceHostDelegateReplicaTemplateConfigurator>().Object
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostAspNetCoreListenerReplicaTemplate()
                           .UseCommunicationListener((
                                context,
                                s,
                                arg3) => new Mock<AspNetCoreCommunicationListener>().Object),
                        new Mock<IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator>().Object
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostRemotingListenerReplicaTemplate()
                           .UseImplementation(() => new Mock<ConfigurableObjectDependenciesConfiguratorTests.IDependencyService>().Object),
                        new Mock<IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator>().Object
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Data), MemberType = typeof(DataSource))]
        public void
            Should_call_all_actions_configured_by_ConfigureObject_When_upstreaming_configuration<TObject, TConfigurator>(
                TObject configurableObject,
                TConfigurator configurator)
            where TObject : class, IConfigurableObject<object>
            where TConfigurator : class
        {
            // Arrange
            var one = new Mock<Action<object>>();
            one.Setup(instance => instance(configurator));

            var two = new Mock<Action<object>>();
            two.Setup(instance => instance(configurator));

            var three = new Mock<Action<object>>();
            three.Setup(instance => instance(configurator));

            // Act
            configurableObject.ConfigureObject(one.Object);
            configurableObject.ConfigureObject(two.Object);
            configurableObject.ConfigureObject(three.Object);

            configurableObject.UpstreamConfiguration(configurator);

            // Assert
            one.Verify(instance => instance(configurator), Times.Once());

            two.Verify(instance => instance(configurator), Times.Once());

            three.Verify(instance => instance(configurator), Times.Once());
        }
    }
}