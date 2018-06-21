using System;
using System.Collections.Generic;
using System.Threading;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Configurators
{
    public class ServiceHostDelegateReplicaTemplateConfiguratorTests
    {
        private static class DataSource
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[]
                    {
                        new StatefulServiceHostDelegateReplicaTemplate(),
                        new Action<StatefulServiceHostDelegateReplicaTemplate>(
                            c =>
                            {
                                c.Activate(Tools.StatefulService).InvokeAsync(CancellationToken.None);
                            })
                    };
                    yield return new object[]
                    {
                        new StatelessServiceHostDelegateReplicaTemplate(),
                        new Action<StatelessServiceHostDelegateReplicaTemplate>(
                            c =>
                            {
                                c.Activate(Tools.StatelessService).InvokeAsync(CancellationToken.None);
                            })
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(DataSource.Data), MemberType = typeof(DataSource))]
        public void Should_use_delegate_invoker_by_UseDelegateInvoker_with_delegate_from_UseDelegate_When_configuring_delegate<TBuilder>(
            TBuilder configurableObject,
            Action<TBuilder> invoke)
            where TBuilder : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            // Arrange
            var root = new Action(
                () =>
                {
                });

            object expectedDelegate = root;
            object actualDelegate = null;

            var invoker = new Mock<IServiceHostDelegateInvoker>();
            invoker
               .Setup(instance => instance.InvokeAsync(It.IsAny<CancellationToken>()))
               .Verifiable();

            // Act
            configurableObject.ConfigureObject(
                config =>
                {
                    config.UseDelegateInvoker(
                        (
                            @delegate,
                            services) =>
                        {
                            actualDelegate = @delegate;
                            return invoker.Object;
                        });
                    config.UseDelegate(root);
                });

            invoke(configurableObject);

            // Assert
            invoker.Verify();

            Assert.Same(expectedDelegate, actualDelegate);
        }
    }
}