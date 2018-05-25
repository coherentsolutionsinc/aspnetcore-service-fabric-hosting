using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Fabric
{
    public class ServiceHostRunnerTests
    {
        [Fact]
        public void
            Should_run_host_When_running_runner_instance()
        {
            // Arrange
            var host = new Mock<IServiceHost>();

            // Act
            var runner = new ServiceHostRunner(host.Object);
            runner.Run();

            // Assert
            host.Verify(instance => instance.Run(), Times.Once());
        }
    }
}