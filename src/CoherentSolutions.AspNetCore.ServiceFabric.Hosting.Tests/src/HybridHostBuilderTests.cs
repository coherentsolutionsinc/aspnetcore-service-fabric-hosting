using System;
using System.Collections.Generic;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common.Exceptions;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;

using Microsoft.AspNetCore.Hosting;

using Moq;

using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests
{
    public class HybridHostBuilderTests
    {
        private class MinimalStartup
        {
            public void Configure()
            {
            }
        }

        // Bug: https://github.com/coherentsolutionsinc/aspnetcore-service-fabric-hosting/issues/6
        [Fact]
        public void
            Should_build_IHost_When_HybridHostBuilder_configuring_web_host_with_minimal_configuration()
        {
            new HybridHostBuilder()
               .ConfigureWebHost(
                    b =>
                    {
                        b.UseStartup<MinimalStartup>();
                    })
               .Build();
        }

        [Fact]
        public void
            Should_throw_FactoryProducesNullInstanceException_When_stateful_service_host_builder_func_returns_null()
        {
            // Arrange
            var factory = new Mock<Func<IStatefulServiceHostBuilder>>();

            var config = new Mock<Action<IStatefulServiceHostBuilder>>();

            var builder = new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseStatefulServiceHostBuilder(factory.Object)
               .ConfigureStatefulServiceHost(config.Object);

            // Act, Assert
            Assert.Throws<FactoryProducesNullInstanceException<IStatefulServiceHostBuilder>>(() => builder.Build());

            factory.Verify(instance => instance(), Times.Once());

            config.Verify(instance => instance(It.IsAny<IStatefulServiceHostBuilder>()), Times.Never());
        }

        [Fact]
        public void
            Should_throw_FactoryProducesNullInstanceException_When_stateless_service_host_builder_func_returns_null()
        {
            // Arrange
            var factory = new Mock<Func<IStatelessServiceHostBuilder>>();

            var config = new Mock<Action<IStatelessServiceHostBuilder>>();

            var builder = new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseStatelessServiceHostBuilder(factory.Object)
               .ConfigureStatelessServiceHost(config.Object);

            // Act, Assert
            Assert.Throws<FactoryProducesNullInstanceException<IStatelessServiceHostBuilder>>(() => builder.Build());

            factory.Verify(instance => instance(), Times.Once());

            config.Verify(instance => instance(It.IsAny<IStatelessServiceHostBuilder>()), Times.Never());
        }

        [Fact]
        public void
            Should_throw_FactoryProducesNullInstanceException_When_web_host_builder_func_returns_null()
        {
            // Arrange
            var factory = new Mock<Func<IWebHostBuilder>>();

            var builder = new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseWebHostBuilder(factory.Object)
               .ConfigureDefaultWebHost();

            // Act, Assert
            Assert.Throws<FactoryProducesNullInstanceException<IWebHostBuilder>>(() => builder.Build());

            factory.Verify(instance => instance(), Times.Once());
        }

        [Fact]
        public void
            Should_throw_FactoryProducesNullInstanceException_When_web_host_extensions_func_returns_null()
        {
            // Arrange
            var factory = new Mock<Func<IWebHostExtensionsImpl>>();

            var config = new Mock<Action<IWebHostBuilder>>();

            var builder = new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .Configure(c => c.UseWebHostExtensionsImpl(factory.Object))
               .ConfigureWebHost(config.Object);

            // Act, Assert
            Assert.Throws<FactoryProducesNullInstanceException<IWebHostExtensionsImpl>>(() => builder.Build());

            factory.Verify(instance => instance(), Times.Once());

            config.Verify(instance => instance(It.IsAny<IWebHostBuilder>()), Times.Never());
        }

        [Fact]
        public void
            Should_throw_InvalidOperationException_When_host_selector_returns_null_descriptor()
        {
            // Arrange
            var selector = new Mock<IHostSelector>();

            // Act
            var builder = new HybridHostBuilder()
               .UseHostSelector(() => selector.Object);

            // Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());

            selector.Verify(
                instance => instance.Select(
                    It.IsAny<IEnumerable<IHostKeywordsProvider>>(),
                    It.IsAny<IEnumerable<IHostDescriptor>>()),
                Times.Once());
        }

        [Fact]
        public void
            Should_use_custom_host_selector_When_host_selector_func_is_configured()
        {
            // Arrange
            var factory = new Mock<Func<IHostSelector>>();
            factory
               .Setup(instance => instance())
               .Returns(new HostSelectorStub());

            // Act
            new HybridHostBuilder()
               .UseHostSelector(factory.Object)
               .Build();

            // Assert
            factory.Verify(instance => instance(), Times.Once());
        }

        [Fact]
        public void
            Should_use_custom_stateful_service_host_builder_When_stateful_service_host_builder_func_is_configured()
        {
            // Arrange
            var factory = new Mock<Func<IStatefulServiceHostBuilder>>();
            factory
               .Setup(instance => instance())
               .Returns(new StatefulServiceHostBuilderStub());

            // Act
            new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseStatefulServiceHostBuilder(factory.Object)
               .ConfigureStatefulServiceHost(
                    c =>
                    {
                    })
               .Build();

            // Assert
            factory.Verify(instance => instance(), Times.Once());
        }

        [Fact]
        public void
            Should_use_custom_stateless_service_host_builder_When_stateless_service_host_builder_func_is_configured()
        {
            // Arrange
            var factory = new Mock<Func<IStatelessServiceHostBuilder>>();
            factory
               .Setup(instance => instance())
               .Returns(new StatelessServiceHostBuilderStub());

            // Act
            new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseStatelessServiceHostBuilder(factory.Object)
               .ConfigureStatelessServiceHost(
                    c =>
                    {
                    })
               .Build();

            // Assert
            factory.Verify(instance => instance(), Times.Once());
        }

        [Fact]
        public void
            Should_use_custom_web_host_builder_When_web_host_builder_func_is_configured()
        {
            // Arrange
            var factory = new Mock<Func<IWebHostBuilder>>();
            factory
               .Setup(instance => instance())
               .Returns(new WebHostBuilderStub());

            // Act
            new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseWebHostBuilder(factory.Object)
               .ConfigureDefaultWebHost()
               .Build();

            // Assert
            factory.Verify(instance => instance(), Times.Once());
        }

        [Fact]
        public void
            Should_use_stateful_service_configuration_action_When_configuring_stateful_service()
        {
            // Arrange
            var action = new Mock<Action<IStatefulServiceHostBuilder>>();

            // Act
            new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .ConfigureStatefulServiceHost(action.Object)
               .Build();

            // Assert
            action.Verify(instance => instance(It.IsAny<IStatefulServiceHostBuilder>()), Times.Once());
        }

        [Fact]
        public void
            Should_use_stateless_service_configuration_action_When_configuring_stateless_service()
        {
            // Arrange
            var action = new Mock<Action<IStatelessServiceHostBuilder>>();

            // Act
            new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .ConfigureStatelessServiceHost(action.Object)
               .Build();

            // Assert
            action.Verify(instance => instance(It.IsAny<IStatelessServiceHostBuilder>()), Times.Once());
        }

        [Fact]
        public void
            Should_use_web_host_builder_extension_impl_func_from_host_When_configuring_stateful_service_without_web_host_builder_extension_impl_func()
        {
            // Arrange
            var factory = new Mock<Func<IWebHostBuilderExtensionsImpl>>();

            var configurator = new Mock<IStatefulServiceHostBuilderConfigurator>();

            var builder = new Mock<IStatefulServiceHostBuilder>();
            builder
               .Setup(instance => instance.ConfigureObject(It.IsAny<Action<IStatefulServiceHostBuilderConfigurator>>()))
               .Callback<Action<IStatefulServiceHostBuilderConfigurator>>(action => action(configurator.Object));
            builder
               .Setup(instance => instance.Build())
               .Returns(new Mock<IStatefulServiceHost>().Object);

            // Act
            new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseWebHostBuilder(WebHostBuilderStub.Func)
               .UseStatefulServiceHostBuilder(() => builder.Object)
               .Configure(config => config.UseWebHostBuilderExtensionsImpl(factory.Object))
               .ConfigureStatefulServiceHost(
                    config =>
                    {
                    })
               .Build();

            // Assert
            configurator.Verify(instance => instance.UseWebHostBuilderExtensionsImpl(factory.Object), Times.Once());
        }

        [Fact]
        public void
            Should_use_web_host_builder_extension_impl_func_from_host_When_configuring_stateless_service_without_web_host_builder_extension_impl_func()
        {
            // Arrange
            var factory = new Mock<Func<IWebHostBuilderExtensionsImpl>>();

            var configurator = new Mock<IStatelessServiceHostBuilderConfigurator>();

            var builder = new Mock<IStatelessServiceHostBuilder>();
            builder
               .Setup(instance => instance.ConfigureObject(It.IsAny<Action<IStatelessServiceHostBuilderConfigurator>>()))
               .Callback<Action<IStatelessServiceHostBuilderConfigurator>>(action => action(configurator.Object));
            builder
               .Setup(instance => instance.Build())
               .Returns(new Mock<IStatelessServiceHost>().Object);

            // Act
            new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseWebHostBuilder(WebHostBuilderStub.Func)
               .UseStatelessServiceHostBuilder(() => builder.Object)
               .Configure(config => config.UseWebHostBuilderExtensionsImpl(factory.Object))
               .ConfigureStatelessServiceHost(
                    config =>
                    {
                    })
               .Build();

            // Assert
            configurator.Verify(instance => instance.UseWebHostBuilderExtensionsImpl(factory.Object), Times.Once());
        }

        [Fact]
        public void
            Should_use_web_host_builder_func_from_host_When_configuring_stateful_service_without_web_host_builder_func()
        {
            // Arrange
            var factory = new Mock<Func<IWebHostBuilder>>();

            var configurator = new Mock<IStatefulServiceHostBuilderConfigurator>();

            var builder = new Mock<IStatefulServiceHostBuilder>();
            builder
               .Setup(instance => instance.ConfigureObject(It.IsAny<Action<IStatefulServiceHostBuilderConfigurator>>()))
               .Callback<Action<IStatefulServiceHostBuilderConfigurator>>(action => action(configurator.Object));
            builder
               .Setup(instance => instance.Build())
               .Returns(new Mock<IStatefulServiceHost>().Object);

            // Act
            new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseWebHostBuilder(factory.Object)
               .UseStatefulServiceHostBuilder(() => builder.Object)
               .ConfigureStatefulServiceHost(
                    config =>
                    {
                    })
               .Build();

            // Assert
            configurator.Verify(instance => instance.UseWebHostBuilder(factory.Object), Times.Once());
        }

        [Fact]
        public void
            Should_use_web_host_builder_func_from_host_When_configuring_stateless_service_without_web_host_builder_func()
        {
            // Arrange
            var factory = new Mock<Func<IWebHostBuilder>>();

            var configurator = new Mock<IStatelessServiceHostBuilderConfigurator>();

            var builder = new Mock<IStatelessServiceHostBuilder>();
            builder
               .Setup(instance => instance.ConfigureObject(It.IsAny<Action<IStatelessServiceHostBuilderConfigurator>>()))
               .Callback<Action<IStatelessServiceHostBuilderConfigurator>>(action => action(configurator.Object));
            builder
               .Setup(instance => instance.Build())
               .Returns(new Mock<IStatelessServiceHost>().Object);

            // Act
            new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseWebHostBuilder(factory.Object)
               .UseStatelessServiceHostBuilder(() => builder.Object)
               .ConfigureStatelessServiceHost(
                    config =>
                    {
                    })
               .Build();

            // Assert
            configurator.Verify(instance => instance.UseWebHostBuilder(factory.Object), Times.Once());
        }

        [Fact]
        public void
            Should_use_web_host_configuration_action_When_configuring_web_host()
        {
            // Arrange
            var action = new Mock<Action<IWebHostBuilder>>();
            action
               .Setup(instance => instance(It.IsAny<IWebHostBuilder>()));

            // Act
            new HybridHostBuilder()
               .UseHostSelector(HostSelectorStub.Func)
               .UseWebHostBuilder(WebHostBuilderStub.Func)
               .ConfigureWebHost(action.Object)
               .Build();

            // Assert
            action.Verify(instance => instance(It.IsAny<IWebHostBuilder>()), Times.Once());
        }
    }
}