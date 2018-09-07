using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class StatefulServiceHostListenerReplicatorTests
        : ServiceHostListenerReplicatorTests<IStatefulServiceHostListenerReplicableTemplate, IStatefulService, ServiceReplicaListener>
    {
        protected override ServiceHostListenerReplicator<IStatefulServiceHostListenerReplicableTemplate, IStatefulService, ServiceReplicaListener>
            CreateReplicatorInstance(
                IStatefulServiceHostListenerReplicableTemplate replicableTemplate)
        {
            return new StatefulServiceHostListenerReplicator(replicableTemplate);
        }
    }

    public class StatelessServiceHostListenerReplicatorTests
        : ServiceHostListenerReplicatorTests<IStatelessServiceHostListenerReplicableTemplate, IStatelessService, ServiceInstanceListener>
    {
        protected override ServiceHostListenerReplicator<IStatelessServiceHostListenerReplicableTemplate, IStatelessService, ServiceInstanceListener>
            CreateReplicatorInstance(
                IStatelessServiceHostListenerReplicableTemplate replicableTemplate)
        {
            return new StatelessServiceHostListenerReplicator(replicableTemplate);
        }
    }

    public abstract class ServiceHostListenerReplicatorTests<TReplicableTemplate, TService, TListener>
        where TReplicableTemplate : class, IServiceHostListenerReplicableTemplate<TService, TListener>
        where TService : class
        where TListener : class
    {
        protected abstract ServiceHostListenerReplicator<TReplicableTemplate, TService, TListener> CreateReplicatorInstance(
            TReplicableTemplate replicableTemplate);

        [Fact]
        private void Should_activate_replicable_template_When_replicating_for_service()
        {
            // Arrange
            var mockService = new Mock<TService>();

            var mockReplicableTemplate = new Mock<TReplicableTemplate>();
            mockReplicableTemplate
               .Setup(instance => instance.Activate(mockService.Object))
               .Verifiable();

            var arrangeService = mockService.Object;
            var arrangeReplicableTemplate = mockReplicableTemplate.Object;

            var arrangeReplicator = this.CreateReplicatorInstance(arrangeReplicableTemplate);

            // Act
            arrangeReplicator.ReplicateFor(arrangeService);

            // Assert
            mockReplicableTemplate.Verify();
            mockReplicableTemplate.VerifyNoOtherCalls();
        }
    }
}