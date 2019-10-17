using System;
using System.Collections.Concurrent;
using System.Fabric;
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
                        mockEvent,
                        mockDelegate,
                        () =>
                        {
                            var mockTask = new TaskCompletionSource<bool>();
                            var mockDelegateInvoker = new Mock<IServiceDelegateInvoker>();
                            mockDelegateInvoker
                               .Setup(instance => instance.InvokeAsync(It.IsAny<Delegate>(), It.IsAny<IStatelessServiceDelegateInvocationContext>(), It.IsAny<CancellationToken>()))
                               .Callback(
                                    () =>
                                    {
                                        mockDelegate();
                                        mockTask.SetResult(true);
                                    })
                               .Returns(mockTask.Task);

                            return mockDelegateInvoker.Object;
                        }));

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
        public static async Task Should_invoke_delegates_On_stateless_service_shutdown_cycle()
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
        public static async Task Should_invoke_delegates_On_stateless_service_startup_cycle()
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

        [Fact]
        public static void Should_invoke_packages_events_On_stateless_service_package_activation_events()
        {
            // Arrange
            var actualCallStack = new ConcurrentQueue<string>();

            var mockCodePackageActivationContext = new Mock<ICodePackageActivationContext>();

            var mockDelegateCodePackageAdded = new Mock<Action>();
            mockDelegateCodePackageAdded
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("codepackage.added"))
               .Verifiable();

            var mockDelegateCodePackageModified = new Mock<Action>();
            mockDelegateCodePackageModified
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("codepackage.modified"))
               .Verifiable();

            var mockDelegateCodePackageRemoved = new Mock<Action>();
            mockDelegateCodePackageRemoved
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("codepackage.removed"))
               .Verifiable();

            var mockDelegateConfigPackageAdded = new Mock<Action>();
            mockDelegateConfigPackageAdded
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("configpackage.added"))
               .Verifiable();

            var mockDelegateConfigPackageModified = new Mock<Action>();
            mockDelegateConfigPackageModified
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("configpackage.modified"))
               .Verifiable();

            var mockDelegateConfigPackageRemoved = new Mock<Action>();
            mockDelegateConfigPackageRemoved
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("configpackage.removed"))
               .Verifiable();

            var mockDelegateDataPackageAdded = new Mock<Action>();
            mockDelegateDataPackageAdded
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("datapackage.added"))
               .Verifiable();

            var mockDelegateDataPackageModified = new Mock<Action>();
            mockDelegateDataPackageModified
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("datapackage.modified"))
               .Verifiable();

            var mockDelegateDataPackageRemoved = new Mock<Action>();
            mockDelegateDataPackageRemoved
               .Setup(instance => instance())
               .Callback(() => actualCallStack.Enqueue("datapackage.removed"))
               .Verifiable();

            var mockEventSourceReplicator = MockStatefulServiceHostEventSourceReplicator();
            var mockDelegateReplicators = new[]
            {
                MockDelegateReplicatorForEvent(
                    mockDelegateCodePackageAdded.Object,
                    StatelessServiceLifecycleEvent.OnCodePackageAdded),
                MockDelegateReplicatorForEvent(
                    mockDelegateCodePackageModified.Object,
                    StatelessServiceLifecycleEvent.OnCodePackageModified),
                MockDelegateReplicatorForEvent(
                    mockDelegateCodePackageRemoved.Object,
                    StatelessServiceLifecycleEvent.OnCodePackageRemoved),
                MockDelegateReplicatorForEvent(
                    mockDelegateConfigPackageAdded.Object,
                    StatelessServiceLifecycleEvent.OnConfigPackageAdded),
                MockDelegateReplicatorForEvent(
                    mockDelegateConfigPackageModified.Object,
                    StatelessServiceLifecycleEvent.OnConfigPackageModified),
                MockDelegateReplicatorForEvent(
                    mockDelegateConfigPackageRemoved.Object,
                    StatelessServiceLifecycleEvent.OnConfigPackageRemoved),
                MockDelegateReplicatorForEvent(
                    mockDelegateDataPackageAdded.Object,
                    StatelessServiceLifecycleEvent.OnDataPackageAdded),
                MockDelegateReplicatorForEvent(
                    mockDelegateDataPackageModified.Object,
                    StatelessServiceLifecycleEvent.OnDataPackageModified),
                MockDelegateReplicatorForEvent(
                    mockDelegateDataPackageRemoved.Object,
                    StatelessServiceLifecycleEvent.OnDataPackageRemoved)
            };

            var statelessContext = MockStatelessServiceContextFactory.Create(
                mockCodePackageActivationContext.Object,
                "Mock",
                new Uri("fabric:/mock"),
                Guid.NewGuid(),
                0);

            var statelessInstance = new StatelessService(
                statelessContext,
                mockEventSourceReplicator,
                mockDelegateReplicators,
                Array.Empty<IStatelessServiceHostListenerReplicator>());

            // Act
            mockCodePackageActivationContext
               .Raise(instance => instance.CodePackageAddedEvent -= null, new PackageAddedEventArgs<CodePackage>());
            mockCodePackageActivationContext
               .Raise(instance => instance.CodePackageModifiedEvent -= null, new PackageModifiedEventArgs<CodePackage>());
            mockCodePackageActivationContext
               .Raise(instance => instance.CodePackageRemovedEvent -= null, new PackageRemovedEventArgs<CodePackage>());

            mockCodePackageActivationContext
               .Raise(instance => instance.ConfigurationPackageAddedEvent -= null, new PackageAddedEventArgs<ConfigurationPackage>());
            mockCodePackageActivationContext
               .Raise(instance => instance.ConfigurationPackageModifiedEvent -= null, new PackageModifiedEventArgs<ConfigurationPackage>());
            mockCodePackageActivationContext
               .Raise(instance => instance.ConfigurationPackageRemovedEvent -= null, new PackageRemovedEventArgs<ConfigurationPackage>());

            mockCodePackageActivationContext
               .Raise(instance => instance.DataPackageAddedEvent -= null, new PackageAddedEventArgs<DataPackage>());
            mockCodePackageActivationContext
               .Raise(instance => instance.DataPackageModifiedEvent -= null, new PackageModifiedEventArgs<DataPackage>());
            mockCodePackageActivationContext
               .Raise(instance => instance.DataPackageRemovedEvent -= null, new PackageRemovedEventArgs<DataPackage>());

            // Assert
            mockDelegateCodePackageAdded.Verify();
            mockDelegateCodePackageModified.Verify();
            mockDelegateCodePackageRemoved.Verify();
            mockDelegateConfigPackageAdded.Verify();
            mockDelegateConfigPackageModified.Verify();
            mockDelegateConfigPackageRemoved.Verify();
            mockDelegateDataPackageAdded.Verify();
            mockDelegateDataPackageModified.Verify();
            mockDelegateDataPackageRemoved.Verify();

            Assert.Equal(9, actualCallStack.Count);

            Assert.Equal(
                "codepackage.added",
                actualCallStack.TryDequeue(out var result)
                    ? result
                    : null);

            Assert.Equal(
                "codepackage.modified",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "codepackage.removed",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "configpackage.added",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "configpackage.modified",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "configpackage.removed",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "datapackage.added",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "datapackage.modified",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "datapackage.removed",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);
        }
    }
}