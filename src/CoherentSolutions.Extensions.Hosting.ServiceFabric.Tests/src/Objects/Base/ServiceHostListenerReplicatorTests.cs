using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects.Base
{
    public abstract class ServiceHostListenerReplicatorTests<TReplicableTemplate, TService, TListener>
        where TReplicableTemplate : class, IServiceHostListenerReplicableTemplate<TService, TListener>
        where TService : class
        where TListener : class
    {
        protected abstract ServiceHostListenerReplicator<TReplicableTemplate, TService, TListener> CreateInstance(
            TReplicableTemplate replicableTemplate);

        [Fact]
        public void
            Should_activate_replicable_template_with_service_When_replicating_listeners_for_service()
        {
            // Arrange
            var service = new Mock<TService>();

            var replicableTemplate = new Mock<TReplicableTemplate>();
            replicableTemplate.Setup(instance => instance.Activate(service.Object));

            // Act
            var listenerReplicator = this.CreateInstance(replicableTemplate.Object);
            listenerReplicator.ReplicateFor(service.Object);

            // Assert
            replicableTemplate.Verify(instance => instance.Activate(service.Object), Times.Once);
        }
    }
}