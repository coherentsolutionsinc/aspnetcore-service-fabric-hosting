using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Fabric
{
    public class ServiceHostKeywordsTests
    {
        [Fact]
        public void
            Should_return_aspnetcore_and_servicefabric_keywords_When_getting_keywords()
        {
            // Arrange
            var keywords = new ServiceHostKeywords();

            // Act
            var items = keywords.GetKeywords();

            // Assert
            Assert.Contains(HostKeywords.ENVIRONMENT_ASPNET_CORE, items);
            Assert.Contains(HostKeywords.ENVIRONMENT_SERVICE_FABRIC, items);
        }
    }
}