using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Web
{
    public class WebHostKeywordsTests
    {
        [Fact]
        public void
            Should_return_aspnetcore_keyword_When_getting_keywords()
        {
            // Arrange
            var keywords = new WebHostKeywords();

            // Act, Assert
            Assert.Contains(HostKeywords.ENVIRONMENT_ASPNET_CORE, keywords.GetKeywords());
        }
    }
}