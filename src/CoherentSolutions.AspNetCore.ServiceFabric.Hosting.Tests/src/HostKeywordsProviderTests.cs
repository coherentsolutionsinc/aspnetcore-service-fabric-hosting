using System.Linq;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests
{
    public class HostKeywordsProviderTests
    {
        [Fact]
        public void
            Should_return_aspnetcore_keyword_When_no_environment_detected()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();

            var keywordsProvider = new HostKeywordsProvider(configuration.Object);

            // Act
            var keywords = keywordsProvider.GetKeywords();

            // Assert
            Assert.Contains(HostKeywords.ENVIRONMENT_ASPNET_CORE, keywords);
        }

        [Fact]
        public void
            Should_return_servicefabric_and_aspnetcore_keywords_When_servicefabric_environment_detected()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            configuration
               .Setup(instance => instance[It.IsAny<string>()])
               .Returns("MyApplication");

            var keywordsProvider = new HostKeywordsProvider(configuration.Object);

            // Act
            var keywords = keywordsProvider.GetKeywords().ToArray();

            // Assert
            configuration.Verify(instance => instance[It.Is<string>(v => v == "Fabric_ApplicationName")], Times.Once());

            Assert.Contains(HostKeywords.ENVIRONMENT_SERVICE_FABRIC, keywords);
            Assert.Contains(HostKeywords.ENVIRONMENT_ASPNET_CORE, keywords);
        }
    }
}