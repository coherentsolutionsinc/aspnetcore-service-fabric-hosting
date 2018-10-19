using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

using Moq;

using ServiceFabric.Mocks;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features
{
    public static class StatelessServiceLifecycleTests
    {
        private static IStatelessServiceHostEventSourceReplicator MockStatefulServiceHostEventSourceReplicator()
        {
            var mockEventSourceReplicator = new Mock<IStatelessServiceHostEventSourceReplicator>();
            mockEventSourceReplicator
               .Setup(instance => instance.ReplicateFor(It.IsAny<IStatelessServiceInformation>()))
               .Returns(new StatelessServiceEventSource(() => new Mock<IServiceEventSource>().Object));

            return mockEventSourceReplicator.Object;
        }

        private static IStatelessServiceHostDelegateReplicator MockDelegateReplicatorForEvent(
            Action mockDelegate,
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
                            mockDelegateInvoker
                               .Setup(instance => instance.InvokeAsync(It.IsAny<IStatelessServiceDelegateInvocationContext>(), It.IsAny<CancellationToken>()))
                               .Callback(
                                    () =>
                                    {
                                        mockDelegate();
                                        mockTask.SetResult(true);
                                    })
                               .Returns(mockTask.Task);

                            return mockDelegateInvoker.Object;
                        },
                        mockEvent));

            return mockDelegateReplicator.Object;
        }

        private static IStatelessServiceHostListenerReplicator MockListenerReplicator(
            Action openAsyncDelegate = null,
            Action closeAsyncDelegate = null)
        {
            var mockReplicator = new Mock<IStatelessServiceHostListenerReplicator>();
            mockReplicator.Setup(instance => instance.ReplicateFor(It.IsAny<IStatelessService>()))
               .Returns(
                    new ServiceInstanceListener(
                        context =>
                        {
                            var mockCommunicationListener = new Mock<ICommunicationListener>();
                            mockCommunicationListener
                               .Setup(instance => instance.OpenAsync(It.IsAny<CancellationToken>()))
                               .Callback(() => openAsyncDelegate?.Invoke())
                               .Returns(Task.FromResult(string.Empty));

                            mockCommunicationListener
                               .Setup(instance => instance.CloseAsync(It.IsAny<CancellationToken>()))
                               .Callback(() => closeAsyncDelegate?.Invoke())
                               .Returns(Task.CompletedTask);

                            return mockCommunicationListener.Object;
                        }));

            return mockReplicator.Object;
        }

        [Fact]
        private static async Task Should_invoke_delegates_On_stateless_service_shutdown_cycle()
        {
            // Arrange
            var actualCallStack = new ConcurrentQueue<string>();

            var mockDelegateServiceShutdown = new Mock<Action>();
            mockDelegateServiceShutdown
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.Shutdown"))
               .Verifiable();

            var mockDelegateListenerClose = new Mock<Action>();
            mockDelegateListenerClose
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("listener.Close"))
               .Verifiable();

            var mockEventSourceReplicator = MockStatefulServiceHostEventSourceReplicator();
            var mockDelegateReplicators = new[]
            {
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceShutdown.Object,
                    StatelessServiceLifecycleEvent.OnShutdown),
            };
            var mockListenerReplicators = new[]
            {
                MockListenerReplicator(
                    closeAsyncDelegate: mockDelegateListenerClose.Object)
            };

            var statelessService = new MockStatelessServiceInstance(
                new StatelessService(
                    MockStatelessServiceContextFactory.Default,
                    mockEventSourceReplicator,
                    mockDelegateReplicators,
                    mockListenerReplicators));

            // Act
            await statelessService.InitiateShutdownSequenceAsync();

            // Assert
            mockDelegateServiceShutdown.Verify();

            mockDelegateListenerClose.Verify();

            Assert.Equal(2, actualCallStack.Count);

            Assert.Equal(
                "listener.Close",
                actualCallStack.TryDequeue(out var result)
                    ? result
                    : null);

            Assert.Equal(
                "service.Shutdown",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);
        }

        [Fact]
        private static async Task Should_invoke_delegates_On_stateless_service_startup_cycle()
        {
            // Arrange
            var actualCallStack = new ConcurrentQueue<string>();

            var mockDelegateServiceStartup = new Mock<Action>();
            mockDelegateServiceStartup
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.Startup"))
               .Verifiable();

            var mockDelegateServiceRun = new Mock<Action>();
            mockDelegateServiceRun
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.Run"))
               .Verifiable();

            var mockDelegateListenerOpen = new Mock<Action>();
            mockDelegateListenerOpen
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("listener.Open"))
               .Verifiable();

            var mockEventSourceReplicator = MockStatefulServiceHostEventSourceReplicator();
            var mockDelegateReplicators = new[]
            {
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceStartup.Object,
                    StatelessServiceLifecycleEvent.OnStartup),
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceRun.Object,
                    StatelessServiceLifecycleEvent.OnRun)
            };

            var mockListenerReplicators = new[]
            {
                MockListenerReplicator(
                    mockDelegateListenerOpen.Object)
            };

            var statelessInstance = new MockStatelessServiceInstance(
                new StatelessService(
                    MockStatelessServiceContextFactory.Default,
                    mockEventSourceReplicator,
                    mockDelegateReplicators,
                    mockListenerReplicators));

            // Act
            await statelessInstance.InitiateStartupSequenceAsync();

            // Assert
            mockDelegateServiceStartup.Verify();
            mockDelegateServiceRun.Verify();

            mockDelegateListenerOpen.Verify();

            Assert.Equal(3, actualCallStack.Count);

            Assert.Equal(
                "service.Startup",
                actualCallStack.TryDequeue(out var result)
                    ? result
                    : null);

            Assert.Equal(
                "listener.Open",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "service.Run",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);
        }
    }
}