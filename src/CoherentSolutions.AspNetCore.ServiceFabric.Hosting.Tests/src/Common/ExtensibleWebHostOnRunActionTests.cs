using System;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Common
{
    public class ExtensibleWebHostOnRunActionTests
    {
        [Fact]
        public void
            Should_invoke_action_with_service_provider_from_successor_web_host_instance_When_executing()
        {
            // Arrange
            var serviceProvider = new Mock<IServiceProvider>();

            var action = new Mock<Action<IServiceProvider>>();
            action
               .Setup(instance => instance(serviceProvider.Object));

            var webHost = new Mock<IWebHost>();
            webHost
               .Setup(instance => instance.Services)
               .Returns(serviceProvider.Object);

            // Act
            var extension = new ExtensibleWebHostOnRunAction(action.Object);
            extension.Execute(webHost.Object);

            // Assert
            action.Verify(instance => instance(serviceProvider.Object));
        }
    }
}