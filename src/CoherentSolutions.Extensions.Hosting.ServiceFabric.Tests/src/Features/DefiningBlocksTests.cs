using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Features
{
    public static class DefiningBlocksTests
    {
        private static class Theories
        {
            public class Case : IXunitSerializable
            {
                public TheoryItemPromise Promise { get; private set; }

                public Case()
                {
                }

                public Case(
                    TheoryItemPromise theoryItem)
                {
                    this.Promise = theoryItem;
                }

                public override string ToString()
                {
                    return this.Promise.ToString();
                }

                public void Deserialize(
                    IXunitSerializationInfo info)
                {
                    this.Promise = info.GetValue<TheoryItemPromise>(nameof(this.Promise));
                }

                public void Serialize(
                    IXunitSerializationInfo info)
                {
                    info.AddValue(nameof(this.Promise), this.Promise);
                }
            }

            public static IEnumerable<object[]> AllDelegateCases
            {
                get
                {
                    foreach (var item in TheoryItemsSet.DelegateItems)
                    {
                        yield return new object[]
                        {
                            new Case(item)
                        };
                    }
                }
            }

            public static IEnumerable<object[]> AllListenerCases
            {
                get
                {
                    foreach (var item in TheoryItemsSet.AllListenerItems)
                    {
                        yield return new object[]
                        {
                            new Case(item)
                        };
                    }
                }
            }

            public static IEnumerable<object[]> AspNetCoreListenerCases
            {
                get
                {
                    foreach (var item in TheoryItemsSet.AspNetCoreListenerItems)
                    {
                        yield return new object[]
                        {
                            new Case(item)
                        };
                    }
                }
            }

            public static IEnumerable<object[]> RemotingListenerCases
            {
                get
                {
                    foreach (var item in TheoryItemsSet.RemotingListenerItems)
                    {
                        yield return new object[]
                        {
                            new Case(item)
                        };
                    }
                }
            }

            public static IEnumerable<object[]> GenericListenerCases
            {
                get
                {
                    foreach (var item in TheoryItemsSet.GenericListenerItems)
                    {
                        yield return new object[]
                        {
                            new Case(item)
                        };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(Theories.AllDelegateCases), MemberType = typeof(Theories))]
        private static void Should_invoke_delegate_On_default_service_lifecycle_event(
            Theories.Case @case)
        {
            // Arrange
            var mockDelegate = new Mock<Action>();
            mockDelegate
               .Setup(instance => instance())
               .Verifiable();

            var arrangeDelegate = mockDelegate.Object;

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseDelegateTheoryExtension().Setup(arrangeDelegate));
            theoryItem.Try();

            // Assert
            mockDelegate.Verify();
        }

        [Theory]
        [MemberData(nameof(Theories.AllListenerCases), MemberType = typeof(Theories))]
        private static void Should_use_endpoint_name_For_communication_listener(
            Theories.Case @case)
        {
            // Arrange
            const string ArrangeEndpoint = "Endpoint";

            object expectedEndpoint = ArrangeEndpoint;
            object actualEndpoint = null;

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseListenerEndpointTheoryExtension().Setup(ArrangeEndpoint));
            theoryItem.SetupExtension(new PickListenerEndpointTheoryExtension().Setup(s => actualEndpoint = s));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedEndpoint, actualEndpoint);
        }

        [Theory]
        [MemberData(nameof(Theories.AspNetCoreListenerCases), MemberType = typeof(Theories))]
        private static void Should_use_custom_web_host_builder_For_web_server(
            Theories.Case @case)
        {
            // Arrange
            var mockWebHost = new Mock<IWebHost>();

            var mockWebHostBuilder = new Mock<IWebHostBuilder>();
            mockWebHostBuilder
               .Setup(instance => instance.Build())
               .Returns(mockWebHost.Object)
               .Verifiable();

            var arrangeWebHostBuilder = mockWebHostBuilder.Object;

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseAspNetCoreListenerWebHostBuilderTheoryExtension().Setup(() => arrangeWebHostBuilder));
            theoryItem.Try();

            // Assert
            mockWebHostBuilder.Verify();
        }

        [Theory]
        [MemberData(nameof(Theories.AspNetCoreListenerCases), MemberType = typeof(Theories))]
        private static void Should_use_custom_aspnetcore_communication_listener_For_communication_listener(
            Theories.Case @case)
        {
            // Arrange
            Mock<AspNetCoreCommunicationListener> mockListener = null;

            AspNetCoreCommunicationListener ArrangeListenerFactory(
                ServiceContext context,
                string s,
                Func<string, AspNetCoreCommunicationListener, IWebHost> arg3)
            {
                var action = new Mock<Func<string, AspNetCoreCommunicationListener, IWebHost>>();

                mockListener = new Mock<AspNetCoreCommunicationListener>(context, action.Object);
                mockListener.Setup(instance => instance.OpenAsync(It.IsAny<CancellationToken>()))
                   .Callback(() => arg3(string.Empty, mockListener.Object))
                   .Returns(Task.FromResult(string.Empty));

                return mockListener.Object;
            }

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseAspNetCoreListenerCommunicationListenerTheoryExtension().Setup(ArrangeListenerFactory));
            theoryItem.Try();

            // Assert
            mockListener.Verify();
        }

        [Theory]
        [MemberData(nameof(Theories.RemotingListenerCases), MemberType = typeof(Theories))]
        private static void Should_use_custom_remoting_communication_listener_For_communication_listener(
            Theories.Case @case)
        {
            // Arrange
            Mock<FabricTransportServiceRemotingListener> mockListener = null;

            FabricTransportServiceRemotingListener ArrangeListenerFactory(
                ServiceContext context,
                ServiceHostRemotingCommunicationListenerComponentsFactory build)
            {
                var options = build(context);

                mockListener = new Mock<FabricTransportServiceRemotingListener>(
                    context,
                    options.MessageHandler,
                    options.ListenerSettings,
                    options.MessageSerializationProvider);

                mockListener
                   .As<ICommunicationListener>()
                   .Setup(instance => instance.OpenAsync(It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(string.Empty));

                return mockListener.Object;
            }

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseRemotingListenerCommunicationListenerTheoryExtension().Setup(ArrangeListenerFactory));
            theoryItem.Try();

            // Assert
            mockListener.Verify();
        }

        [Theory]
        [MemberData(nameof(Theories.GenericListenerCases), MemberType = typeof(Theories))]
        private static void Should_use_custom_generic_communication_listener_For_communication_listener(
            Theories.Case @case)
        {
            // Arrange
            Mock<ICommunicationListener> mockListener = null;

            ICommunicationListener ArrangeListenerFactory(
                ServiceContext context,
                string endpointName,
                IServiceProvider provider)
            {
                mockListener = new Mock<ICommunicationListener>();
                mockListener
                   .As<ICommunicationListener>()
                   .Setup(instance => instance.OpenAsync(It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(string.Empty));

                return mockListener.Object;
            }

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseGenericListenerCommunicationListenerTheoryExtension().Setup(ArrangeListenerFactory));
            theoryItem.Try();

            // Assert
            mockListener.Verify();
        }

        [Theory]
        [MemberData(nameof(Theories.RemotingListenerCases), MemberType = typeof(Theories))]
        private static void Should_inject_remoting_implementation_dependencies_When_remoting_implementation_type_is_set(
            Theories.Case @case)
        {
            // Arrange
            var arrangeDependency = new TestDependency();

            var arrangeCollection = new ServiceCollection();
            arrangeCollection.AddSingleton<ITestDependency>(arrangeDependency);

            object expectedDependency = arrangeDependency;
            object actualDependency = null;

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseDependenciesTheoryExtension().Setup(() => arrangeCollection));
            theoryItem.SetupExtension(new UseRemotingListenerImplementationTheoryExtension().Setup<TestRemotingImplementationWithParameters>());
            theoryItem.SetupExtension(
                new PickRemotingListenerImplementationTheoryExtension().Setup(
                    s =>
                    {
                        var impl = (TestRemotingImplementationWithParameters) s;
                        actualDependency = impl.Dependency;
                    }));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedDependency, actualDependency);
        }

        [Theory]
        [MemberData(nameof(Theories.RemotingListenerCases), MemberType = typeof(Theories))]
        private static void Should_inject_remoting_serialization_provider_dependencies_When_remoting_serialization_provider_type_is_set(
            Theories.Case @case)
        {
            // Arrange
            var arrangeDependency = new TestDependency();

            var arrangeCollection = new ServiceCollection();
            arrangeCollection.AddSingleton<ITestDependency>(arrangeDependency);

            object expectedDependency = arrangeDependency;
            object actualDependency = null;

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseDependenciesTheoryExtension().Setup(() => arrangeCollection));
            theoryItem.SetupExtension(new UseRemotingListenerSerializationProviderTheoryExtension().Setup<TestRemotingSerializationProviderWithParameters>());
            theoryItem.SetupExtension(
                new PickRemotingListenerSerializationProviderTheoryExtension().Setup(
                    s =>
                    {
                        var impl = (TestRemotingSerializationProviderWithParameters) s;
                        actualDependency = impl.Dependency;
                    }));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedDependency, actualDependency);
        }

        [Theory]
        [MemberData(nameof(Theories.RemotingListenerCases), MemberType = typeof(Theories))]
        private static void Should_inject_remoting_handler_dependencies_When_remoting_handler_type_is_set(
            Theories.Case @case)
        {
            // Arrange
            var arrangeDependency = new TestDependency();

            var arrangeCollection = new ServiceCollection();
            arrangeCollection.AddSingleton<ITestDependency>(arrangeDependency);

            object expectedDependency = arrangeDependency;
            object actualDependency = null;

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseDependenciesTheoryExtension().Setup(() => arrangeCollection));
            theoryItem.SetupExtension(new UseRemotingListenerHandlerTheoryExtension().Setup<TestRemotingHandlerWithParameters>());
            theoryItem.SetupExtension(
                new PickRemotingListenerHandlerTheoryExtension().Setup(
                    s =>
                    {
                        var impl = (TestRemotingHandlerWithParameters) s;
                        actualDependency = impl.Dependency;
                    }));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedDependency, actualDependency);
        }

        [Theory]
        [MemberData(nameof(Theories.RemotingListenerCases), MemberType = typeof(Theories))]
        private static void Should_use_custom_remoting_implementation_For_communication_listener(
            Theories.Case @case)
        {
            // Arrange
            var arrangeImplementation = new TestRemotingImplementation();

            object expectedImplementation = arrangeImplementation;
            object actualImplementation = null;

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseRemotingListenerImplementationTheoryExtension().Setup(provider => arrangeImplementation));
            theoryItem.SetupExtension(new PickRemotingListenerImplementationTheoryExtension().Setup(s => actualImplementation = s));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedImplementation, actualImplementation);
        }

        [Theory]
        [MemberData(nameof(Theories.RemotingListenerCases), MemberType = typeof(Theories))]
        private static void Should_use_custom_remoting_settings_For_communication_listener(
            Theories.Case @case)
        {
            // Arrange
            var arrangeSettings = new FabricTransportRemotingListenerSettings();

            object expectedSettings = arrangeSettings;
            object actualSettings = null;

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseRemotingListenerSettingsTheoryExtension().Setup(() => arrangeSettings));
            theoryItem.SetupExtension(new PickRemotingListenerSettingsTheoryExtension().Setup(s => actualSettings = s));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedSettings, actualSettings);
        }

        [Theory]
        [MemberData(nameof(Theories.RemotingListenerCases), MemberType = typeof(Theories))]
        private static void Should_use_custom_remoting_serialization_provider_For_communication_listener(
            Theories.Case @case)
        {
            // Arrange
            var mockSerializer = new Mock<IServiceRemotingMessageSerializationProvider>();

            var arrangeSerializer = mockSerializer.Object;

            object expectedSerializer = arrangeSerializer;
            object actualSerializer = null;

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseRemotingListenerSerializationProviderTheoryExtension().Setup(provider => arrangeSerializer));
            theoryItem.SetupExtension(new PickRemotingListenerSerializationProviderTheoryExtension().Setup(s => actualSerializer = s));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedSerializer, actualSerializer);
        }

        [Theory]
        [MemberData(nameof(Theories.RemotingListenerCases), MemberType = typeof(Theories))]
        private static void Should_use_custom_remoting_handler_For_communication_listener(
            Theories.Case @case)
        {
            // Arrange
            var mockHandler = new Mock<IServiceRemotingMessageHandler>();

            var arrangeHandler = mockHandler.Object;

            object expectedHandler = arrangeHandler;
            object actualHandler = null;

            var theoryItem = @case.Promise.Resolve();

            // Act
            theoryItem.SetupExtension(new UseRemotingListenerHandlerTheoryExtension().Setup(provider => arrangeHandler));
            theoryItem.SetupExtension(new PickRemotingListenerHandlerTheoryExtension().Setup(s => actualHandler = s));
            theoryItem.Try();

            // Assert
            Assert.Same(expectedHandler, actualHandler);
        }

        [Fact]
        private static void Should_use_delegate_invoker_When_invoking_delegates_on_stateful_service_lifecycle_events()
        {
            // Arrange
            var mockDelegateInvoker = new Mock<IServiceHostDelegateInvoker<IStatefulServiceDelegateInvocationContext>>();
            mockDelegateInvoker
               .Setup(instance => instance.InvokeAsync(It.IsAny<IStatefulServiceDelegateInvocationContext>(), It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask)
               .Verifiable();

            var arrangeDelegateInvoker = mockDelegateInvoker.Object;

            var theoryItem = TheoryItemsSet.StatefulServiceDelegate.Resolve();

            // Act
            theoryItem.SetupExtension(
                new UseStatefulDelegateInvokerTheoryExtension().Setup(
                    (
                        @delegate,
                        provider) => arrangeDelegateInvoker));

            theoryItem.Try();

            // Assert
            mockDelegateInvoker.Verify();
        }

        [Fact]
        private static void Should_use_delegate_invoker_When_invoking_delegates_on_stateless_service_lifecycle_events()
        {
            // Arrange
            var mockDelegateInvoker = new Mock<IServiceHostDelegateInvoker<IStatelessServiceDelegateInvocationContext>>();
            mockDelegateInvoker
               .Setup(instance => instance.InvokeAsync(It.IsAny<IStatelessServiceDelegateInvocationContext>(), It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask)
               .Verifiable();

            var arrangeDelegateInvoker = mockDelegateInvoker.Object;

            var theoryItem = TheoryItemsSet.StatelessServiceDelegate.Resolve();

            // Act
            theoryItem.SetupExtension(
                new UseStatelessDelegateInvokerTheoryExtension().Setup(
                    (
                        @delegate,
                        provider) => arrangeDelegateInvoker));

            theoryItem.Try();

            // Assert
            mockDelegateInvoker.Verify();
        }
    }
}