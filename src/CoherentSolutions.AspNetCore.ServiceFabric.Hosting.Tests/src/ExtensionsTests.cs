using System;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests
{
    public class ExtensionsTests
    {
        [Fact]
        public void
            Should_register_on_run_action_implementation_on_successor_When_ConfigureOnRun_is_called_on_ExtensibleWebHostBuilder_instance()
        {
            // Arrange
            var successorServiceCollection = new Mock<IServiceCollection>();

            var successor = new Mock<IWebHostBuilder>();
            successor
               .Setup(instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
               .Returns(successor.Object)
               .Callback<Action<IServiceCollection>>(action => action(successorServiceCollection.Object));

            // Act
            var extensibleBuilder = new ExtensibleWebHostBuilder(successor.Object);
            extensibleBuilder.ConfigureOnRun(
                serviceProvider =>
                {
                });

            // Assert
            successor.Verify(
                instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()),
                Times.Once());

            successorServiceCollection.Verify(
                instance => instance.Add(
                    It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IExtensibleWebHostOnRunAction))),
                Times.Once());
        }

        [Fact]
        public void
            Should_throw_InvalidOperationException_When_ConfigureOnRun_is_called_on_non_ExtensibleWebHostBuilder_instance()
        {
            // Arrange
            var instance = new Mock<IWebHostBuilder>();

            // Act, Assert
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    instance.Object.ConfigureOnRun(
                        serviceProvider =>
                        {
                        });
                });
        }
    }
}