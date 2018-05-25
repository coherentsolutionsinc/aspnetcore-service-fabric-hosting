using System;
using System.Linq.Expressions;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Common
{
    public class ExtensibleWebHostBuilderTests
    {
        [Fact]
        public void
            Should_build_successor_When_building()
        {
            // Arrange
            var successorWebHost = new Mock<IWebHost>();

            var successor = new Mock<IWebHostBuilder>();
            successor
               .Setup(instance => instance.Build())
               .Returns(successorWebHost.Object);

            // Act
            var extensibleWebHost = new ExtensibleWebHostBuilder(successor.Object);

            extensibleWebHost.Build();

            // Assert
            successor.Verify(instance => instance.Build(), Times.Once());
        }

        [Fact]
        public void
            Should_configure_app_configuration_on_successor_When_configuring_app_configuration()
        {
            // Arrange
            var successor = new Mock<IWebHostBuilder>();
            successor
               .Setup(
                    instance
                        => instance.ConfigureAppConfiguration(
                            It.IsAny<Action<WebHostBuilderContext, IConfigurationBuilder>>()))
               .Returns(successor.Object);

            // Act
            var extensibleWebHost = new ExtensibleWebHostBuilder(successor.Object);
            extensibleWebHost.ConfigureAppConfiguration(
                (
                    context,
                    config) =>
                {
                });

            // Assert
            successor.Verify(
                instance
                    => instance.ConfigureAppConfiguration(
                        It.IsAny<Action<WebHostBuilderContext, IConfigurationBuilder>>()),
                Times.Once());
        }

        [Fact]
        public void
            Should_configure_services_on_successor_When_configuring_services()
        {
            // Arrange
            var successor = new Mock<IWebHostBuilder>();

            // Act
            var extensibleWebHost = new ExtensibleWebHostBuilder(successor.Object);
            extensibleWebHost.ConfigureServices(
                services =>
                {
                });
            extensibleWebHost.ConfigureServices(
                (
                    context,
                    services) =>
                {
                });

            // Assert
            successor.Verify(
                instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()),
                Times.Once());

            successor.Verify(
                instance => instance.ConfigureServices(It.IsAny<Action<WebHostBuilderContext, IServiceCollection>>()),
                Times.Once());
        }

        [Fact]
        public void
            Should_create_instance_of_ExtensibleWebHost_When_building()
        {
            // Arrange
            var successorWebHost = new Mock<IWebHost>();
            var successor = new Mock<IWebHostBuilder>();
            successor
               .Setup(instance => instance.Build())
               .Returns(successorWebHost.Object);

            // Act
            var extensibleBuilder = new ExtensibleWebHostBuilder(successor.Object);
            var output = extensibleBuilder.Build();

            // Assert
            Assert.IsAssignableFrom<ExtensibleWebHost>(output);
        }

        [Fact]
        public void
            Should_get_settings_from_successor_When_getting_settings()
        {
            // Arrange
            var value = "my-value";

            Expression<Func<string, bool>> valueArg = v => v == value;

            var successor = new Mock<IWebHostBuilder>();
            successor
               .Setup(instance => instance.GetSetting(It.Is(valueArg)))
               .Returns(value);

            // Act
            var extensibleWebHost = new ExtensibleWebHostBuilder(successor.Object);

            extensibleWebHost.GetSetting(value);

            // Assert
            successor.Verify(instance => instance.GetSetting(It.Is(valueArg)), Times.Once());
        }

        [Fact]
        public void
            Should_return_successor_build_result_When_successor_built_is_ExtensibleWebHost_instance()
        {
            // Arrange
            var successorWebHostRoot = new Mock<IWebHost>();
            var successorWebHost = new Mock<ExtensibleWebHost>(successorWebHostRoot.Object);
            var successor = new Mock<IWebHostBuilder>();
            successor
               .Setup(instance => instance.Build())
               .Returns(successorWebHost.Object);

            // Act
            var extensibleBuilder = new ExtensibleWebHostBuilder(successor.Object);
            var output = extensibleBuilder.Build();

            // Assert
            Assert.Same(successorWebHost.Object, output);
        }

        [Fact]
        public void
            Should_set_settings_to_successor_When_set_settings()
        {
            // Arrange
            var key = "my-key";
            var value = "my-value";

            Expression<Func<string, bool>> keyArg = v => v == key;
            Expression<Func<string, bool>> valueArg = v => v == value;

            var successor = new Mock<IWebHostBuilder>();
            successor
               .Setup(instance => instance.UseSetting(It.Is(keyArg), It.Is(valueArg)))
               .Returns(successor.Object);

            // Act
            var extensibleWebHost = new ExtensibleWebHostBuilder(successor.Object);

            extensibleWebHost.UseSetting(key, value);

            // Assert
            successor.Verify(instance => instance.UseSetting(It.Is(keyArg), It.Is(valueArg)), Times.Once());
        }
    }
}