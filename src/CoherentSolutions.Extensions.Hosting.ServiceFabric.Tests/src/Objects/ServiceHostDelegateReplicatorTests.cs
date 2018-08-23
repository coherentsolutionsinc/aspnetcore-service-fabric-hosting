using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class StatefulServiceHostDelegateReplicatorTests
        : ServiceHostDelegateReplicatorTests<IStatefulServiceHostDelegateReplicableTemplate, IStatefulService, StatefulServiceDelegate>
    {
        protected override ServiceHostDelegateReplicator<IStatefulServiceHostDelegateReplicableTemplate, IStatefulService, StatefulServiceDelegate>
            CreateReplicatorInstance(
                IStatefulServiceHostDelegateReplicableTemplate replicableTemplate)
        {
            return new StatefulServiceHostDelegateReplicator(replicableTemplate);
        }
    }

    public class StatelessServiceHostDelegateReplicatorTests
        : ServiceHostDelegateReplicatorTests<IStatelessServiceHostDelegateReplicableTemplate, IStatelessService, StatelessServiceDelegate>
    {
        protected override ServiceHostDelegateReplicator<IStatelessServiceHostDelegateReplicableTemplate, IStatelessService, StatelessServiceDelegate>
            CreateReplicatorInstance(
                IStatelessServiceHostDelegateReplicableTemplate replicableTemplate)
        {
            return new StatelessServiceHostDelegateReplicator(replicableTemplate);
        }
    }

    public abstract class ServiceHostDelegateReplicatorTests<TReplicableTemplate, TService, TDelegate>
        where TReplicableTemplate : class, IServiceHostDelegateReplicableTemplate<TService, TDelegate>
        where TService : class
        where TDelegate : class
    {
        protected abstract ServiceHostDelegateReplicator<TReplicableTemplate, TService, TDelegate> CreateReplicatorInstance(
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