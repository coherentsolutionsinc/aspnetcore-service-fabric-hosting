using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Moq;

using ServiceFabric.Mocks;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features
{
    public static class LifecycleTests
    {
        [Fact]
        private static async void Should_invoke_delegates_On_stateless_service_startup_cycle()
        {
            // Arrange
            var expectedCallStack = new Stack<StatelessServiceLifecycleEvent>(
                new[]
                {
                    StatelessServiceLifecycleEvent.OnRunBeforeListenersOpened,
                    StatelessServiceLifecycleEvent.OnRunAfterListenersOpened,
                    StatelessServiceLifecycleEvent.OnOpen
                });
            var actualCallStack = new Stack<StatelessServiceLifecycleEvent>();

            var mockDelegateOnOpen = new Mock<Action>();
            mockDelegateOnOpen
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Push(StatelessServiceLifecycleEvent.OnOpen))
               .Verifiable();

            var mockDelegateOnRunBeforeListenersOpened = new Mock<Action>();
            mockDelegateOnRunBeforeListenersOpened
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Push(StatelessServiceLifecycleEvent.OnRunBeforeListenersOpened))
               .Verifiable();

            var mockDelegateOnRunAfterListenersOpened = new Mock<Action>();
            mockDelegateOnRunAfterListenersOpened
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Push(StatelessServiceLifecycleEvent.OnRunAfterListenersOpened))
               .Verifiable();

            var mockDelegateReplicators = new[]
            {
                MockStatelessServiceHostDelegateReplicatorForEvent(mockDelegateOnOpen, StatelessServiceLifecycleEvent.OnOpen),
                MockStatelessServiceHostDelegateReplicatorForEvent(mockDelegateOnRunBeforeListenersOpened, StatelessServiceLifecycleEvent.OnRunBeforeListenersOpened),
                MockStatelessServiceHostDelegateReplicatorForEvent(mockDelegateOnRunAfterListenersOpened, StatelessServiceLifecycleEvent.OnRunAfterListenersOpened)
            };

            var statelessService = new MockStatelessServiceInstance(
                context => new StatelessService(context, mockDelegateReplicators, null),
                MockStatelessServiceContextFactory.Default);

            // Act
            await statelessService.CreateAsync();
            await statelessService.DestroyAsync();

            // Assert
            mockDelegateOnOpen.Verify();
            mockDelegateOnRunBeforeListenersOpened.Verify();
            mockDelegateOnRunAfterListenersOpened.Verify();

            Assert.Equal(expectedCallStack, actualCallStack);
        }

        [Fact]
        private static async void Should_invoke_delegates_On_stateless_service_shutdown_cycle()
        {
            // Arrange
            var expectedCallStack = new Stack<StatelessServiceLifecycleEvent>(
                new[]
                {
                    StatelessServiceLifecycleEvent.OnClose
                });
            var actualCallStack = new Stack<StatelessServiceLifecycleEvent>();

            var mockDelegateOnClose = new Mock<Action>();
            mockDelegateOnClose
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Push(StatelessServiceLifecycleEvent.OnClose))
               .Verifiable();

            var mockDelegateReplicators = new[]
            {
                MockStatelessServiceHostDelegateReplicatorForEvent(mockDelegateOnClose, StatelessServiceLifecycleEvent.OnClose),
            };

            var statelessService = new MockStatelessServiceInstance(
                context => new StatelessService(context, mockDelegateReplicators, null),
                MockStatelessServiceContextFactory.Default);

            // Act
            await statelessService.CreateAsync();
            await statelessService.DestroyAsync();

            // Assert
            mockDelegateOnClose.Verify();

            Assert.Equal(expectedCallStack, actualCallStack);
        }

        private static IStatelessServiceHostDelegateReplicator MockStatelessServiceHostDelegateReplicatorForEvent(
            Mock<Action> mockDelegate,
            StatelessServiceLifecycleEvent mockEvent)
        {
            var mockDelegateReplicator = new Mock<IStatelessServiceHostDelegateReplicator>();
            mockDelegateReplicator.Setup(instance => instance.ReplicateFor(It.IsAny<IStatelessService>()))
               .Returns(
                    new StatelessServiceDelegate(
                        () =>
                        {
                            var mockTask = new TaskCompletionSource<bool>();
                            var mockDelegateInvoker = new Mock<IStatelessServiceHostDelegateInvoker>();
                            mockDelegateInvoker.Setup(instance => instance.InvokeAsync(It.IsAny<IStatelessServiceDelegateInvocationContext>(), It.IsAny<CancellationToken>()))
                               .Callback(
                                    () =>
                                    {
                                        mockDelegate.Object();
                                        mockTask.SetResult(true);
                                    })
                               .Returns(mockTask.Task);

                            return mockDelegateInvoker.Object;
                        },
                        mockEvent));

            return mockDelegateReplicator.Object;
        }
    }
}