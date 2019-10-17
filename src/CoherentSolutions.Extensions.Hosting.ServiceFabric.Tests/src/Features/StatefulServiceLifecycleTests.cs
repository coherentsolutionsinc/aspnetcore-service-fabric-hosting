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
    public static class StatefulServiceLifecycleTests
    {
        private static IStatefulServiceHostEventSourceReplicator MockEventSourceReplicator()
        {
            var mockEventSourceReplicator = new Mock<IStatefulServiceHostEventSourceReplicator>();
            mockEventSourceReplicator
               .Setup(instance => instance.ReplicateFor(It.IsAny<IStatefulServiceInformation>()))
               .Returns(new StatefulServiceEventSource(() => new Mock<IServiceEventSource>().Object));

            return mockEventSourceReplicator.Object;
        }

        private static IStatefulServiceHostDelegateReplicator MockDelegateReplicatorForEvent(
            Action mockDelegate,
            StatefulServiceLifecycleEvent mockEvent)
        {
            var mockDelegateReplicator = new Mock<IStatefulServiceHostDelegateReplicator>();
            mockDelegateReplicator.Setup(instance => instance.ReplicateFor(It.IsAny<IStatefulService>()))
               .Returns(
                    new StatefulServiceDelegate(
                        mockEvent,
                        mockDelegate,
                        () =>
                        {
                            var mockTask = new TaskCompletionSource<bool>();
                            var mockDelegateInvoker = new Mock<IServiceDelegateInvoker>();
                            mockDelegateInvoker.Setup(instance => instance.InvokeAsync(It.IsAny<Delegate>(), It.IsAny<IStatefulServiceDelegateInvocationContext>(), It.IsAny<CancellationToken>()))
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
        public static async Task Should_invoke_delegate_On_stateful_service_creation_demotion_promotion_cycle()
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

            var mockEventSourceReplicator = MockEventSourceReplicator();
            var mockDelegateReplicators = new[]
            {
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceStartup.Object,
                    StatefulServiceLifecycleEvent.OnStartup),
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceChangeRole.Object,
                    StatefulServiceLifecycleEvent.OnChangeRole),
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceRun.Object,
                    StatefulServiceLifecycleEvent.OnRun),
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceShutdown.Object,
                    StatefulServiceLifecycleEvent.OnShutdown)
            };
            var mockListenerReplicators = new[]
            {
                MockListenerReplicator(
                    mockDelegateListenerOpen.Object,
                    mockDelegateListenerClose.Object)
            };

            var statefulService = new StatefulService(
                MockStatefulServiceContextFactory.Default,
                mockEventSourceReplicator,
                mockDelegateReplicators,
                mockListenerReplicators);

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
                "service.ChangeRole",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);
            Assert.Equal(
                "service.Run",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            // demote
            Assert.Equal(
                "listener.Close",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "service.ChangeRole",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            // promote
            Assert.Equal(
                "listener.Open",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "service.ChangeRole",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);
            Assert.Equal(
                "service.Run",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            // shutdown
            Assert.Equal(
                "listener.Close",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);

            Assert.Equal(
                "service.ChangeRole",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);
            Assert.Equal(
                "service.Shutdown",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);
        }

        [Fact]
        public static async Task Should_invoke_delegate_On_stateful_service_shutdown_cycle()
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

            var mockEventSourceReplicator = MockEventSourceReplicator();
            var mockDelegateReplicators = new[]
            {
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceShutdown.Object,
                    StatefulServiceLifecycleEvent.OnShutdown),
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceChangeRole.Object,
                    StatefulServiceLifecycleEvent.OnChangeRole)
            };
            var mockListenerReplicators = new[]
            {
                MockListenerReplicator(
                    closeAsyncDelegate: mockDelegateListenerClose.Object)
            };

            var statefulService = new StatefulService(
                MockStatefulServiceContextFactory.Default,
                mockEventSourceReplicator,
                mockDelegateReplicators,
                mockListenerReplicators);

            var statefulReplica = new MockStatefulServiceReplica(statefulService);

            // Act
            await statefulReplica.InitiateShutdownSequenceAsync();

            // Assert
            mockDelegateServiceShutdown.Verify();
            mockDelegateServiceChangeRole.Verify();

            mockDelegateListenerClose.Verify();

            Assert.Equal(3, actualCallStack.Count);

            Assert.Equal(
                "listener.Close",
                actualCallStack.TryDequeue(out var result)
                    ? result
                    : null);

            Assert.Equal(
                "service.ChangeRole",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);
            Assert.Equal(
                "service.Shutdown",
                actualCallStack.TryDequeue(out result)
                    ? result
                    : null);
        }

        [Fact]
        public static async Task Should_invoke_delegate_On_stateful_service_startup_cycle()
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

            var mockEventSourceReplicator = MockEventSourceReplicator();
            var mockDelegateReplicators = new[]
            {
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceStartup.Object,
                    StatefulServiceLifecycleEvent.OnStartup),
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceChangeRoleAsync.Object,
                    StatefulServiceLifecycleEvent.OnChangeRole),
                MockDelegateReplicatorForEvent(
                    mockDelegateServiceRun.Object,
                    StatefulServiceLifecycleEvent.OnRun),
            };
            var mockListenerReplicators = new[]
            {
                MockListenerReplicator(
                    mockDelegateListenerOpen.Object)
            };

            var statefulService = new StatefulService(
                MockStatefulServiceContextFactory.Default,
                mockEventSourceReplicator,
                mockDelegateReplicators,
                mockListenerReplicators);

            var statefulReplica = new MockStatefulServiceReplica(statefulService);

            // Act
            await statefulReplica.InitiateStartupSequenceAsync();

            // Assert
            mockDelegateServiceStartup.Verify();
            mockDelegateServiceChangeRoleAsync.Verify();
            mockDelegateServiceRun.Verify();

            mockDelegateListenerOpen.Verify();

            Assert.Equal(4, actualCallStack.Count);

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
                "service.ChangeRole",
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

            var mockEventSourceReplicator = MockEventSourceReplicator();
            var mockDelegateReplicators = new[]
            {
                MockDelegateReplicatorForEvent(
                    mockDelegateCodePackageAdded.Object,
                    StatefulServiceLifecycleEvent.OnCodePackageAdded),
                MockDelegateReplicatorForEvent(
                    mockDelegateCodePackageModified.Object,
                    StatefulServiceLifecycleEvent.OnCodePackageModified),
                MockDelegateReplicatorForEvent(
                    mockDelegateCodePackageRemoved.Object,
                    StatefulServiceLifecycleEvent.OnCodePackageRemoved),
                MockDelegateReplicatorForEvent(
                    mockDelegateConfigPackageAdded.Object,
                    StatefulServiceLifecycleEvent.OnConfigPackageAdded),
                MockDelegateReplicatorForEvent(
                    mockDelegateConfigPackageModified.Object,
                    StatefulServiceLifecycleEvent.OnConfigPackageModified),
                MockDelegateReplicatorForEvent(
                    mockDelegateConfigPackageRemoved.Object,
                    StatefulServiceLifecycleEvent.OnConfigPackageRemoved),
                MockDelegateReplicatorForEvent(
                    mockDelegateDataPackageAdded.Object,
                    StatefulServiceLifecycleEvent.OnDataPackageAdded),
                MockDelegateReplicatorForEvent(
                    mockDelegateDataPackageModified.Object,
                    StatefulServiceLifecycleEvent.OnDataPackageModified),
                MockDelegateReplicatorForEvent(
                    mockDelegateDataPackageRemoved.Object,
                    StatefulServiceLifecycleEvent.OnDataPackageRemoved)
            };

            var statefulContext = MockStatefulServiceContextFactory.Create(
                mockCodePackageActivationContext.Object,
                "Mock",
                new Uri("fabric:/mock"),
                Guid.NewGuid(),
                0);

            var statefulInstance = new StatefulService(
                statefulContext,
                mockEventSourceReplicator,
                mockDelegateReplicators,
                Array.Empty<IStatefulServiceHostListenerReplicator>());

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