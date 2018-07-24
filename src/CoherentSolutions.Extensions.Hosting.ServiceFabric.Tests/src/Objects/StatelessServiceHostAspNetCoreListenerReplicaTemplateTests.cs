using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class StatelessServiceHostAspNetCoreListenerReplicaTemplateTests
    {
        [Fact]
        public void
            Should_set_listener_name_When_endpoint_is_configured()
        {
            // Arrange
            const string ArrangeName = "value";

            object expectedName = ArrangeName;
            object actualName = null;

            var replicableTemplate = new StatelessServiceHostAspNetCoreListenerReplicaTemplate();
            replicableTemplate.ConfigureObject(
                c =>
                {
                    c.UseEndpoint(ArrangeName);
                });

            // Act
            var listener = replicableTemplate.Activate(Tools.StatelessService);

            actualName = listener.Name;

            // Assert
            Assert.Same(expectedName, actualName);
        }
    }
}