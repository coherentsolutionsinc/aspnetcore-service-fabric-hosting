using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Configurators
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
                        new Action<StatefulServiceHostBuilder>(c => c.Build())
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostDelegateReplicaTemplate()
                           .UseDelegate(() => Task.CompletedTask),
                        new Action<StatefulServiceHostDelegateReplicaTemplate>(c => c.Activate(Tools.StatefulService))
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostAspNetCoreListenerReplicaTemplate()
                           .UseCommunicationListener(
                                (
                                    context,
                                    s,
                                    arg3) => new Mock<AspNetCoreCommunicationListener>(
                                    Tools.StatefulContext,
                                    new Mock<Func<string, AspNetCoreCommunicationListener, IWebHost>>().Object
                                ).Object),
                        new Action<StatefulServiceHostAspNetCoreListenerReplicaTemplate>(
                            c => c.Activate(Tools.StatefulService).CreateCommunicationListener(Tools.StatefulContext))
                    };
                    yield return new object[]
                    {
                        new StatefulServiceHostRemotingListenerReplicaTemplate()
                           .UseImplementation(() => new Mock<ConfigurableObjectDependenciesConfiguratorTests.IDependencyService>().Object),
                        new Action<StatefulServiceHostRemotingListenerReplicaTemplate>(
                            c => c.Activate(Tools.StatefulService).CreateCommunicationListener(Tools.StatefulContext))
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostBuilder(),
                        new Action<StatelessServiceHostBuilder>(c => c.Build())
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostDelegateReplicaTemplate()
                           .UseDelegate(() => Task.CompletedTask),
                        new Action<StatelessServiceHostDelegateReplicaTemplate>(c => c.Activate(Tools.StatelessService))
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostAspNetCoreListenerReplicaTemplate()
                           .UseCommunicationListener(
                                (
                                    context,
                                    s,
                                    arg3) => new Mock<AspNetCoreCommunicationListener>(
                                    Tools.StatelessContext,
                                    new Mock<Func<string, AspNetCoreCommunicationListener, IWebHost>>().Object
                                ).Object),
                        new Action<StatelessServiceHostAspNetCoreListenerReplicaTemplate>(
                            c => c.Activate(Tools.StatelessService).CreateCommunicationListener(Tools.StatelessContext))
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostRemotingListenerReplicaTemplate()
                           .UseImplementation(() => new Mock<ConfigurableObjectDependenciesConfiguratorTests.IDependencyService>().Object),
                        new Action<StatelessServiceHostRemotingListenerReplicaTemplate>(
                            c => c.Activate(Tools.StatelessService).CreateCommunicationListener(Tools.StatelessContext))
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Data), MemberType = typeof(DataSource))]
        public void
            Should_call_all_actions_configured_by_ConfigureObject_When_upstreaming_configuration<TObject, TConfigurator>(
                TObject configurableObject,
                Action<TObject> invoke)
            where TObject : class, IConfigurableObject<object>
            where TConfigurator : class
        {
            // Arrange
            var @delegate = new Mock<Action<object>>();
            @delegate.Setup(instance => instance(It.IsAny<TConfigurator>()));

            // Act
            configurableObject.ConfigureObject(@delegate.Object);
            configurableObject.ConfigureObject(@delegate.Object);
            configurableObject.ConfigureObject(@delegate.Object);

            invoke(configurableObject);

            // Assert
            @delegate.Verify(instance => instance(It.IsAny<TConfigurator>()), Times.Exactly(3));
        }
    }
}