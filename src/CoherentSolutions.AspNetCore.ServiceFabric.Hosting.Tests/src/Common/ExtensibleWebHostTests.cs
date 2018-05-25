using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Common
{
    public class ExtensibleWebHostTests
    {
        [Fact]
        public void
            Should_dispose_successor_method_When_disposing()
        {
            // Arrange
            var successor = new Mock<IWebHost>();
            successor
               .Setup(instance => instance.Dispose());

            // Act
            var extensibleWebHost = new ExtensibleWebHost(successor.Object);

            extensibleWebHost.Dispose();

            // Assert
            successor.Verify(instance => instance.Dispose(), Times.Once());
        }

        [Fact]
        public void
            Should_execute_on_run_action_implementation_When_starting_asynchronously()
        {
            // Arrange
            Expression<Func<Type, bool>> serviceTypeArg = v => v == typeof(IEnumerable<IExtensibleWebHostOnRunAction>);

            var sequence = new MockSequence();

            var onRunAction = new Mock<IExtensibleWebHostOnRunAction>();
            onRunAction
               .InSequence(sequence)
               .Setup(instance => instance.Execute(It.IsNotNull<IWebHost>()));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
               .Setup(instance => instance.GetService(It.Is(serviceTypeArg)))
               .Returns<Type>(t => new[] { onRunAction.Object });

            var successor = new Mock<IWebHost>();
            successor
               .Setup(instance => instance.Services)
               .Returns(serviceProvider.Object);
            successor
               .InSequence(sequence)
               .Setup(instance => instance.StartAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            // Act
            var extensibleWebHost = new ExtensibleWebHost(successor.Object);
            extensibleWebHost.StartAsync();

            // Assert
            successor.Verify(instance => instance.StartAsync(It.IsAny<CancellationToken>()), Times.Once());

            serviceProvider.Verify(instance => instance.GetService(It.Is(serviceTypeArg)), Times.Once());

            onRunAction.Verify(instance => instance.Execute(extensibleWebHost), Times.Once());
        }

        [Fact]
        public void
            Should_execute_on_run_action_implementation_When_starting_synchronously()
        {
            // Arrange
            Expression<Func<Type, bool>> serviceTypeArg = v => v == typeof(IEnumerable<IExtensibleWebHostOnRunAction>);

            var sequence = new MockSequence();

            var onRunAction = new Mock<IExtensibleWebHostOnRunAction>();
            onRunAction
               .InSequence(sequence)
               .Setup(instance => instance.Execute(It.IsNotNull<IWebHost>()));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
               .Setup(instance => instance.GetService(It.Is(serviceTypeArg)))
               .Returns<Type>(t => new[] { onRunAction.Object });

            var successor = new Mock<IWebHost>();
            successor
               .Setup(instance => instance.Services)
               .Returns(serviceProvider.Object);
            successor
               .InSequence(sequence)
               .Setup(instance => instance.Start());

            // Act
            var extensibleWebHost = new ExtensibleWebHost(successor.Object);
            extensibleWebHost.Start();

            // Assert
            successor.Verify(instance => instance.Start(), Times.Once());

            serviceProvider.Verify(instance => instance.GetService(It.Is(serviceTypeArg)), Times.Once());

            onRunAction.Verify(instance => instance.Execute(extensibleWebHost), Times.Once());
        }

        [Fact]
        public void
            Should_proxy_successor_server_features_When_getting_server_features()
        {
            // Arrange
            var successorsServerFeatures = new Mock<IFeatureCollection>(MockBehavior.Loose);

            var successor = new Mock<IWebHost>();
            successor
               .Setup(instance => instance.ServerFeatures)
               .Returns(successorsServerFeatures.Object);

            // Act
            var extensibleWebHost = new ExtensibleWebHost(successor.Object);

            var serverFeatures = extensibleWebHost.ServerFeatures;

            // Assert
            Assert.Same(successorsServerFeatures.Object, serverFeatures);

            successor.Verify(instance => instance.ServerFeatures, Times.Once());
        }

        [Fact]
        public void
            Should_proxy_successor_services_When_getting_services()
        {
            // Arrange
            var successorsServiceProvider = new Mock<IServiceProvider>();

            var successor = new Mock<IWebHost>();
            successor
               .Setup(instance => instance.Services)
               .Returns(successorsServiceProvider.Object);

            // Act
            var extensibleWebHost = new ExtensibleWebHost(successor.Object);

            var serviceProvider = extensibleWebHost.Services;

            // Assert
            Assert.Same(successorsServiceProvider.Object, serviceProvider);

            successor.Verify(instance => instance.Services, Times.Once());
        }

        [Fact]
        public void
            Should_start_successor_asynchronously_When_starting_asynchronously()
        {
            // Arrange
            var successorsServiceProvider = new Mock<IServiceProvider>();
            successorsServiceProvider
               .Setup(
                    instance
                        => instance.GetService(
                            It.Is<Type>(v => v == typeof(IEnumerable<IExtensibleWebHostOnRunAction>))))
               .Returns(Enumerable.Empty<IExtensibleWebHostOnRunAction>());

            var successor = new Mock<IWebHost>();
            successor
               .Setup(instance => instance.Services)
               .Returns(successorsServiceProvider.Object);
            successor
               .Setup(instance => instance.StartAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            // Act
            var cancellationToken = new CancellationToken(true);
            var extensibleWebHost = new ExtensibleWebHost(successor.Object);

            extensibleWebHost.StartAsync(cancellationToken);

            // Assert
            successor.Verify(instance => instance.Services, Times.Once());
            successor.Verify(
                instance => instance.StartAsync(It.Is<CancellationToken>(v => v.IsCancellationRequested)),
                Times.Once());
        }

        [Fact]
        public void
            Should_start_successor_synchronously_When_starting_synchronously()
        {
            // Arrange
            var successorsServiceProvider = new Mock<IServiceProvider>();
            successorsServiceProvider
               .Setup(
                    instance
                        => instance.GetService(
                            It.Is<Type>(v => v == typeof(IEnumerable<IExtensibleWebHostOnRunAction>))))
               .Returns(Enumerable.Empty<IExtensibleWebHostOnRunAction>());

            var successor = new Mock<IWebHost>();
            successor
               .Setup(instance => instance.Services)
               .Returns(successorsServiceProvider.Object);
            successor
               .Setup(instance => instance.Start());

            // Act
            var extensibleWebHost = new ExtensibleWebHost(successor.Object);

            extensibleWebHost.Start();

            // Assert
            successor.Verify(instance => instance.Services, Times.Once());
            successor.Verify(instance => instance.Start(), Times.Once());
        }

        [Fact]
        public void
            Should_stop_successor_asynchronously_When_stopping_asynchronously()
        {
            // Arrange
            var successor = new Mock<IWebHost>();
            successor
               .Setup(instance => instance.StopAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            // Act
            var cancellationToken = new CancellationToken(true);
            var extensibleWebHost = new ExtensibleWebHost(successor.Object);

            extensibleWebHost.StopAsync(cancellationToken);

            // Assert
            successor.Verify(
                instance => instance.StopAsync(It.Is<CancellationToken>(v => v.IsCancellationRequested)),
                Times.Once());
        }
    }
}