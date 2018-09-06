using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public static class StatefulServiceLifecycleTests
    {
        private static IStatefulServiceHostDelegateReplicator MockStatefulServiceHostDelegateReplicatorForEvent(
            Action mockDelegate,
            StatefulServiceLifecycleEvent mockEvent)
        {
            var mockDelegateReplicator = new Mock<IStatefulServiceHostDelegateReplicator>();
            mockDelegateReplicator.Setup(instance => instance.ReplicateFor(It.IsAny<IStatefulService>()))
               .Returns(
                    new StatefulServiceDelegate(
                        () =>
                        {
                            var mockTask = new TaskCompletionSource<bool>();
                            var mockDelegateInvoker = new Mock<IStatefulServiceHostDelegateInvoker>();
                            mockDelegateInvoker.Setup(
                                    instance => instance.InvokeAsync(It.IsAny<IStatefulServiceDelegateInvocationContext>(), It.IsAny<CancellationToken>()))
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

        private static IStatefulServiceHostListenerReplicator MockListenerReplicator(
            Action openAsyncDelegate = null,
            Action closeAsyncDelegate = null)
        {
            var mockReplicator = new Mock<IStatefulServiceHostListenerReplicator>();
            mockReplicator.Setup(instance => instance.ReplicateFor(It.IsAny<IStatefulService>()))
               .Returns(
                    new ServiceReplicaListener(
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
        private static async Task Should_invoke_delegate_On_stateful_service_creation_demotion_promotion_cycle()
        {
            // Arrange
            var actualCallStack = new ConcurrentQueue<string>();

            var mockDelegateServiceStartup = new Mock<Action>();
            mockDelegateServiceStartup
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.Startup"))
               .Verifiable();

            var mockDelegateServiceChangeRole = new Mock<Action>();
            mockDelegateServiceChangeRole
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.ChangeRole"))
               .Verifiable();

            var mockDelegateServiceRun = new Mock<Action>();
            mockDelegateServiceRun
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.Run"))
               .Verifiable();

            var mockDelegateServiceShutdown = new Mock<Action>();
            mockDelegateServiceShutdown
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.Shutdown"))
               .Verifiable();

            var mockDelegateListenerOpen = new Mock<Action>();
            mockDelegateListenerOpen
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("listener.Open"))
               .Verifiable();

            var mockDelegateListenerClose = new Mock<Action>();
            mockDelegateListenerClose
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("listener.Close"))
               .Verifiable();

            var mockDelegateReplicators = new[]
            {
                MockStatefulServiceHostDelegateReplicatorForEvent(
                    mockDelegateServiceStartup.Object, 
                    StatefulServiceLifecycleEvent.OnStartup),

                MockStatefulServiceHostDelegateReplicatorForEvent(
                    mockDelegateServiceChangeRole.Object, 
                    StatefulServiceLifecycleEvent.OnChangeRole),

                MockStatefulServiceHostDelegateReplicatorForEvent(
                    mockDelegateServiceRun.Object, 
                    StatefulServiceLifecycleEvent.OnRun),

                MockStatefulServiceHostDelegateReplicatorForEvent(
                    mockDelegateServiceShutdown.Object, 
                    StatefulServiceLifecycleEvent.OnShutdown)
            };
            var mockListenerReplicators = new[]
            {
                MockListenerReplicator(
                    openAsyncDelegate: mockDelegateListenerOpen.Object,
                    closeAsyncDelegate: mockDelegateListenerClose.Object)
            };

            var statefulService = new StatefulService(MockStatefulServiceContextFactory.Default, mockDelegateReplicators, mockListenerReplicators);
            var statefulReplica = new MockStatefulServiceReplica(statefulService);

            // Act
            await statefulReplica.InitiateStartupSequenceAsync();
            await statefulReplica.InitiateDemotionSequenceAsync();
            await statefulReplica.InitiatePromotionSequenceAsync();
            await statefulReplica.InitiateShutdownSequenceAsync();

            // Assert
            mockDelegateServiceStartup.Verify();
            mockDelegateServiceChangeRole.Verify();
            mockDelegateServiceRun.Verify();
            mockDelegateServiceShutdown.Verify();
            mockDelegateServiceChangeRole.Verify();

            mockDelegateListenerOpen.Verify();
            mockDelegateListenerClose.Verify();

            Assert.Equal(12, actualCallStack.Count);

            // startup
            Assert.Equal("service.Startup", actualCallStack.TryDequeue(out var result) ? result : null);

            Assert.Equal("listener.Open", actualCallStack.TryDequeue(out result) ? result : null);

            Assert.Equal("service.ChangeRole", actualCallStack.TryDequeue(out result) ? result : null);
            Assert.Equal("service.Run", actualCallStack.TryDequeue(out result) ? result : null);

            // demote
            Assert.Equal("listener.Close", actualCallStack.TryDequeue(out result) ? result : null);

            Assert.Equal("service.ChangeRole", actualCallStack.TryDequeue(out result) ? result : null);

            // promote
            Assert.Equal("listener.Open", actualCallStack.TryDequeue(out result) ? result : null);

            Assert.Equal("service.ChangeRole", actualCallStack.TryDequeue(out result) ? result : null);
            Assert.Equal("service.Run", actualCallStack.TryDequeue(out result) ? result : null);

            // shutdown
            Assert.Equal("listener.Close", actualCallStack.TryDequeue(out result) ? result : null);

            Assert.Equal("service.ChangeRole", actualCallStack.TryDequeue(out result) ? result : null);
            Assert.Equal("service.Shutdown", actualCallStack.TryDequeue(out result) ? result : null);
        }

        [Fact]
        private static async Task Should_invoke_delegate_On_stateful_service_shutdown_cycle()
        {
            var actualCallStack = new ConcurrentQueue<string>();

            var mockDelegateServiceShutdown = new Mock<Action>();
            mockDelegateServiceShutdown
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.Shutdown"))
               .Verifiable();

            var mockDelegateServiceChangeRole = new Mock<Action>();
            mockDelegateServiceChangeRole
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.ChangeRole"))
               .Verifiable();

            var mockDelegateListenerClose = new Mock<Action>();
            mockDelegateListenerClose
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("listener.Close"))
               .Verifiable();

            var mockDelegateReplicators = new[]
            {
                MockStatefulServiceHostDelegateReplicatorForEvent(
                    mockDelegateServiceShutdown.Object, 
                    StatefulServiceLifecycleEvent.OnShutdown),

                MockStatefulServiceHostDelegateReplicatorForEvent(
                    mockDelegateServiceChangeRole.Object, 
                    StatefulServiceLifecycleEvent.OnChangeRole)
            };
            var mockListenerReplicators = new[]
            {
                MockListenerReplicator(
                    closeAsyncDelegate: mockDelegateListenerClose.Object)
            };

            var statefulService = new StatefulService(MockStatefulServiceContextFactory.Default, mockDelegateReplicators, mockListenerReplicators);
            var statefulReplica = new MockStatefulServiceReplica(statefulService);

            // Act
            await statefulReplica.InitiateShutdownSequenceAsync();

            // Assert
            mockDelegateServiceShutdown.Verify();
            mockDelegateServiceChangeRole.Verify();

            mockDelegateListenerClose.Verify();

            Assert.Equal(3, actualCallStack.Count);

            Assert.Equal("listener.Close", actualCallStack.TryDequeue(out var result) ? result : null);

            Assert.Equal("service.ChangeRole", actualCallStack.TryDequeue(out result) ? result : null);
            Assert.Equal("service.Shutdown", actualCallStack.TryDequeue(out result) ? result : null);
        }

        [Fact]
        private static async Task Should_invoke_delegate_On_stateful_service_startup_cycle()
        {
            // Arrange
            var actualCallStack = new ConcurrentQueue<string>();

            var mockDelegateServiceStartup = new Mock<Action>();
            mockDelegateServiceStartup
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.Startup"))
               .Verifiable();

            var mockDelegateServiceChangeRoleAsync = new Mock<Action>();
            mockDelegateServiceChangeRoleAsync
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("service.ChangeRole"))
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

            var mockDelegateReplicators = new[]
            {
                MockStatefulServiceHostDelegateReplicatorForEvent(
                    mockDelegateServiceStartup.Object, 
                    StatefulServiceLifecycleEvent.OnStartup),

                MockStatefulServiceHostDelegateReplicatorForEvent(
                    mockDelegateServiceChangeRoleAsync.Object, 
                    StatefulServiceLifecycleEvent.OnChangeRole),

                MockStatefulServiceHostDelegateReplicatorForEvent(
                    mockDelegateServiceRun.Object, 
                    StatefulServiceLifecycleEvent.OnRun),
            };
            var mockListenerReplicators = new[]
            {
                MockListenerReplicator(
                    openAsyncDelegate: mockDelegateListenerOpen.Object)
            };

            var statefulService = new StatefulService(MockStatefulServiceContextFactory.Default, mockDelegateReplicators, mockListenerReplicators);
            var statefulReplica = new MockStatefulServiceReplica(statefulService);

            // Act
            await statefulReplica.InitiateStartupSequenceAsync();

            // Assert
            mockDelegateServiceStartup.Verify();
            mockDelegateServiceChangeRoleAsync.Verify();
            mockDelegateServiceRun.Verify();

            mockDelegateListenerOpen.Verify();

            Assert.Equal(4, actualCallStack.Count);

            Assert.Equal("service.Startup", actualCallStack.TryDequeue(out var result) ? result : null);

            Assert.Equal("listener.Open", actualCallStack.TryDequeue(out result) ? result : null);

            Assert.Equal("service.ChangeRole", actualCallStack.TryDequeue(out result) ? result : null);
            Assert.Equal("service.Run", actualCallStack.TryDequeue(out result) ? result : null);
        }
    }
}