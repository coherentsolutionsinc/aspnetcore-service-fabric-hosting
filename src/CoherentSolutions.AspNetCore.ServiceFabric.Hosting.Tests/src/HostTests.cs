using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests
{
    public class HostTests
    {
        [Fact]
        public void
            Should_run_host_When_running_runner()
        {
            // Arrange
            var runner = new Mock<IHostRunner>();

            // Act
            var host = new Host(runner.Object);
            host.Run();

            // Assert
            runner.Verify(instance => instance.Run(), Times.Once());
        }
    }
}