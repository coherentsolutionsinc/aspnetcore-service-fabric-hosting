using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Moq;

using Xunit;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Behaviors
{
    public class ImplOfConfigurableObjectTests
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
                        new Action<StatefulServiceHostBuilder>(c => c.Build())
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
                        new StatefulServiceHostAspNetCoreListenerReplicaTemplate(),
                        new Action<StatefulServiceHostAspNetCoreListenerReplicaTemplate>(
                            c =>
                            {
                                c.UseCommunicationListener(Tools.AspNetCoreCommunicationListenerFunc);
                                c.Activate(Tools.StatefulService).CreateCommunicationListener(Tools.StatefulContext);
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
                        new Action<StatelessServiceHostBuilder>(c => c.Build())
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
                        new StatelessServiceHostAspNetCoreListenerReplicaTemplate(),
                        new Action<StatelessServiceHostAspNetCoreListenerReplicaTemplate>(
                            c =>
                            {
                                c.UseCommunicationListener(Tools.AspNetCoreCommunicationListenerFunc);
                                c.Activate(Tools.StatelessService).CreateCommunicationListener(Tools.StatelessContext);
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