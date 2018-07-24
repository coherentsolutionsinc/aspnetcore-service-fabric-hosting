using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class StatefulServiceHostAspNetCoreListenerReplicaTemplateTests
    {
        [Fact]
        public void
            Should_set_listener_name_When_endpoint_is_configured()
        {
            // Arrange
            const string ArrangeName = "value";

            object expectedName = ArrangeName;
            object actualName = null;

            var replicableTemplate = new StatefulServiceHostAspNetCoreListenerReplicaTemplate();
            replicableTemplate.ConfigureObject(
                c =>
                {
                    c.UseEndpoint(ArrangeName);
                });

            // Act
            var listener = replicableTemplate.Activate(Tools.StatefulService);

            actualName = listener.Name;

            // Assert
            Assert.Same(expectedName, actualName);
        }

        [Fact]
        public void
            Should_set_listenn_on_secondary_When_listen_on_secondary_is_configured()
        {
            // Arrange
            var replicableTemplate = new StatefulServiceHostAspNetCoreListenerReplicaTemplate();
            replicableTemplate.ConfigureObject(
                c =>
                {
                    c.UseListenerOnSecondary();
                });

            // Act
            var listener = replicableTemplate.Activate(Tools.StatefulService);

            // Assert
            Assert.True(listener.ListenOnSecondary);
        }
    }
}