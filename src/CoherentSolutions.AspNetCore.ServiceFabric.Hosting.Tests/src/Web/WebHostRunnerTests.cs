using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Web
{
    public class WebHostRunnerTests
    {
        [Fact]
        public void
            Should_run_host_When_running_runner()
        {
            // Arrange
            var host = new Mock<IWebHost>();

            var impl = new Mock<IWebHostExtensionsImpl>();

            // Act
            new WebHostRunner(host.Object, impl.Object).Run();

            // Assert
            impl.Verify(instance => instance.Run(host.Object), Times.Once());
        }
    }
}