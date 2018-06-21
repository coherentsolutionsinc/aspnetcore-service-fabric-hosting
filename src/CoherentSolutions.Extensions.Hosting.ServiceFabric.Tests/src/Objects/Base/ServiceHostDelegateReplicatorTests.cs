using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects.Base
{
    public abstract class ServiceHostDelegateReplicatorTests<TReplicableTemplate, TService, TDelegate>
        where TReplicableTemplate : class, IServiceHostDelegateReplicableTemplate<TService, TDelegate>
        where TService : class
        where TDelegate : class
    {
        protected abstract ServiceHostDelegateReplicator<TReplicableTemplate, TService, TDelegate> CreateInstance(
            TReplicableTemplate replicableTemplate);

        [Fact]
        public void
            Should_activate_replicable_template_When_replicating()
        {
            // Arrange
            var service = new Mock<TService>();

            var replicableTemplate = new Mock<TReplicableTemplate>();
            replicableTemplate.Setup(instance => instance.Activate(service.Object));

            // Act
            var replicator = this.CreateInstance(replicableTemplate.Object);
            replicator.ReplicateFor(service.Object);

            // Assert
            replicableTemplate.Verify(instance => instance.Activate(service.Object), Times.Once);
        }
    }
}