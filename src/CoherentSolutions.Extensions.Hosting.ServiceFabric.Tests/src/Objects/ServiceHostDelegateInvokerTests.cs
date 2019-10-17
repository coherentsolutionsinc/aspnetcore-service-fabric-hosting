using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.ServiceFabric.Data;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    //public class StatefulServiceHostDelegateInvokerTests : ServiceHostDelegateInvokerTests<IStatefulServiceDelegateInvocationContext>
    //{
    //    protected override IStatefulServiceDelegateInvocationContext CreateInvocationContext()
    //    {
    //        return new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnRun);
    //    }

    //    protected override ServiceDelegateInvoker<IStatefulServiceDelegateInvocationContext> CreateInvokerInstance(
    //        Delegate @delegate,
    //        IServiceProvider services)
    //    {
    //        return new StatefulServiceHostDelegateInvoker(@delegate, services);
    //    }

    //    public static IEnumerable<object[]> DelegateContexts()
    //    {
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnCodePackageAdded(
    //                new ServiceEventPayloadOnPackageAdded<CodePackage>(null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnCodePackageModified(
    //                new ServiceEventPayloadOnPackageModified<CodePackage>(null, null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnCodePackageRemoved(
    //                new ServiceEventPayloadOnPackageRemoved<CodePackage>(null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnConfigPackageAdded(
    //                new ServiceEventPayloadOnPackageAdded<ConfigurationPackage>(null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnConfigPackageModified(
    //                new ServiceEventPayloadOnPackageModified<ConfigurationPackage>(null, null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnConfigPackageRemoved(
    //                new ServiceEventPayloadOnPackageRemoved<ConfigurationPackage>(null)),
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnDataPackageAdded(
    //                new ServiceEventPayloadOnPackageAdded<DataPackage>(null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnDataPackageModified(
    //                new ServiceEventPayloadOnPackageModified<DataPackage>(null, null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnDataPackageRemoved(
    //                new ServiceEventPayloadOnPackageRemoved<DataPackage>(null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnChangeRole(
    //                new StatefulServiceEventPayloadOnChangeRole(ReplicaRole.Primary))
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnDataLoss(
    //                new StatefulServiceEventPayloadOnDataLoss(new StatefulServiceRestoreContext(new RestoreContext())))
    //        };
    //        yield return new object[]
    //        {
    //            new StatefulServiceDelegateInvocationContextOnShutdown(new StatefulServiceEventPayloadOnShutdown(false))
    //        };
    //    }

    //    [Theory]
    //    [MemberData(nameof(DelegateContexts))]
    //    public async Task Should_resolve_on_payload_argument_When_reacting_on_event<TPayload>(
    //        StatefulServiceDelegateInvocationContext<TPayload> context)
    //    {
    //        // Arrange 
    //        var expectedPayload = context.Payload;
    //        TPayload actualPayload = default;

    //        var mockServices = new Mock<IServiceProvider>();

    //        var arrangeInvocationContext = context;
    //        var arrangeServices = mockServices.Object;
    //        var arrangeAction = new Action<TPayload>(payload => actualPayload = payload);

    //        var arrangeInvoker = this.CreateInvokerInstance(arrangeAction, arrangeServices);

    //        // Act
    //        await arrangeInvoker.InvokeAsync(arrangeInvocationContext, CancellationToken.None);

    //        // Assert
    //        mockServices.VerifyNoOtherCalls();

    //        Assert.Equal(expectedPayload, actualPayload);
    //    }
    //}

    //public class StatelessServiceHostDelegateInvokerTests : ServiceHostDelegateInvokerTests<IStatelessServiceDelegateInvocationContext>
    //{
    //    protected override IStatelessServiceDelegateInvocationContext CreateInvocationContext()
    //    {
    //        return new StatelessServiceDelegateInvocationContext(StatelessServiceLifecycleEvent.OnRun);
    //    }

    //    protected override ServiceDelegateInvoker<IStatelessServiceDelegateInvocationContext> CreateInvokerInstance(
    //        Delegate @delegate,
    //        IServiceProvider services)
    //    {
    //        return new StatelessServiceHostDelegateInvoker(@delegate, services);
    //    }

    //    public static IEnumerable<object[]> DelegateContexts()
    //    {
    //        yield return new object[]
    //        {
    //            new StatelessServiceDelegateInvocationContextOnCodePackageAdded(
    //                new ServiceEventPayloadOnPackageAdded<CodePackage>(null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatelessServiceDelegateInvocationContextOnCodePackageModified(
    //                new ServiceEventPayloadOnPackageModified<CodePackage>(null, null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatelessServiceDelegateInvocationContextOnCodePackageRemoved(
    //                new ServiceEventPayloadOnPackageRemoved<CodePackage>(null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatelessServiceDelegateInvocationContextOnConfigPackageAdded(
    //                new ServiceEventPayloadOnPackageAdded<ConfigurationPackage>(null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatelessServiceDelegateInvocationContextOnConfigPackageModified(
    //                new ServiceEventPayloadOnPackageModified<ConfigurationPackage>(null, null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatelessServiceDelegateInvocationContextOnConfigPackageRemoved(
    //                new ServiceEventPayloadOnPackageRemoved<ConfigurationPackage>(null)),
    //        };
    //        yield return new object[]
    //        {
    //            new StatelessServiceDelegateInvocationContextOnDataPackageAdded(
    //                new ServiceEventPayloadOnPackageAdded<DataPackage>(null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatelessServiceDelegateInvocationContextOnDataPackageModified(
    //                new ServiceEventPayloadOnPackageModified<DataPackage>(null, null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatelessServiceDelegateInvocationContextOnDataPackageRemoved(
    //                new ServiceEventPayloadOnPackageRemoved<DataPackage>(null))
    //        };
    //        yield return new object[]
    //        {
    //            new StatelessServiceDelegateInvocationContextOnShutdown(new StatelessServiceEventPayloadOnShutdown(false))
    //        };
    //    }

    //    [Theory]
    //    [MemberData(nameof(DelegateContexts))]
    //    public async Task Should_resolve_on_payload_argument_When_reacting_on_event<TPayload>(
    //        StatelessServiceDelegateInvocationContext<TPayload> context)
    //    {
    //        // Arrange 
    //        var expectedPayload = context.Payload;
    //        TPayload actualPayload = default;

    //        var mockServices = new Mock<IServiceProvider>();

    //        var arrangeInvocationContext = context;
    //        var arrangeServices = mockServices.Object;
    //        var arrangeAction = new Action<TPayload>(payload => actualPayload = payload);

    //        var arrangeInvoker = this.CreateInvokerInstance(arrangeAction, arrangeServices);

    //        // Act
    //        await arrangeInvoker.InvokeAsync(arrangeInvocationContext, CancellationToken.None);

    //        // Assert
    //        mockServices.VerifyNoOtherCalls();

    //        Assert.Equal(expectedPayload, actualPayload);
    //    }
    //}

    public abstract class ServiceHostDelegateInvokerTests
    {
        private interface ITestDependency
        {
        }

        private class TestDependency : ITestDependency
        {
        }

        private class TestException : Exception
        {
        }

        [Fact]
        public async Task Should_resolve_arguments_from_service_provider_When_action_has_parameters()
        {
            // Arrange 
            var arrangeObject = new TestDependency();

            var expectedByType = arrangeObject;

            TestDependency actualByType = null;
            ITestDependency actualByInterface = null;

            var mockInvocationContext = new Mock<IServiceDelegateInvocationContext>();

            var mockServices = new Mock<IServiceProvider>();
            mockServices
               .Setup(instance => instance.GetService(typeof(TestDependency)))
               .Returns(arrangeObject)
               .Verifiable();

            mockServices
               .Setup(instance => instance.GetService(typeof(ITestDependency)))
               .Returns(arrangeObject)
               .Verifiable();

            var arrangeAction = new Action<TestDependency, ITestDependency>(
                (
                    byType,
                    byInterface) =>
                {
                    actualByType = byType;
                    actualByInterface = byInterface;
                });
            var arrangeInvocationContext = mockInvocationContext.Object;
            var arrangeServices = mockServices.Object;
            var arrangeInvoker = new ServiceDelegateInvoker(
                Enumerable.Empty<IServiceDelegateInvocationContextRegistrant>(), 
                arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeAction, arrangeInvocationContext, CancellationToken.None);

            // Assert
            mockServices.Verify();

            Assert.Same(arrangeObject, actualByType);
            Assert.Same(arrangeObject, actualByInterface);
        }

        [Fact]
        public async Task Should_resolve_cancellation_token_When_action_has_cancellation_token_parameter()
        {
            // Arrange 
            var arrangeCancellationTokenSource = new CancellationTokenSource();
            var arrangeCancellationToken = arrangeCancellationTokenSource.Token;

            var mockInvocationContext = new Mock<IServiceDelegateInvocationContext>();
            var mockServices = new Mock<IServiceProvider>();

            CancellationToken actualCancellationToken = default;

            var arrangeAction = new Action<CancellationToken>(
                cancellationToken => actualCancellationToken = cancellationToken);

            var arrangeInvocationContext = mockInvocationContext.Object;
            var arrangeServices = mockServices.Object;
            var arrangeInvoker = new ServiceDelegateInvoker(
                Enumerable.Empty<IServiceDelegateInvocationContextRegistrant>(),
                arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeAction, arrangeInvocationContext, arrangeCancellationToken);

            // Assert
            Assert.Equal(arrangeCancellationToken.GetHashCode(), actualCancellationToken.GetHashCode());
        }

        [Fact]
        public async Task Should_resolve_invocation_context_argument_When_action_has_invocation_context_parameter()
        {
            // Arrange 
            var mockInvocationContext = new Mock<IServiceDelegateInvocationContext>();
            var mockServices = new Mock<IServiceProvider>();

            IServiceDelegateInvocationContext actualInvocationContext = null;

            var arrangeAction = new Action<IServiceDelegateInvocationContext>(
                invocationContext => actualInvocationContext = invocationContext);

            var arrangeInvocationContext = mockInvocationContext.Object;
            var arrangeServices = mockServices.Object;
            var arrangeInvoker = new ServiceDelegateInvoker(
                Enumerable.Empty<IServiceDelegateInvocationContextRegistrant>(),
                arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeAction, arrangeInvocationContext, CancellationToken.None);

            // Assert
            Assert.Same(arrangeInvocationContext, actualInvocationContext);
        }

        [Fact]
        public async Task Should_resolve_context_registrations_When_action_has_such_parameter()
        {
            // Arrange 
            var arrangeObj = new TestDependency();

            var mockInvocationContext = new Mock<IServiceDelegateInvocationContext>();
            var mockInvocationContextRegistrant = new Mock<IServiceDelegateInvocationContextRegistrant>();
            mockInvocationContextRegistrant
                .Setup(instance => instance.GetInvocationContextRegistrations(It.IsAny<IServiceDelegateInvocationContext>()))
                .Returns(new (Type, object)[] { (typeof(TestDependency), arrangeObj) });

            var mockServices = new Mock<IServiceProvider>();

            TestDependency actualObj = null;

            var arrangeAction = new Action<TestDependency>(
                obj => actualObj = obj);

            var arrangeInvocationContext = mockInvocationContext.Object;
            var arrangeServices = mockServices.Object;
            var arrangeInvoker = new ServiceDelegateInvoker(
                new[] { mockInvocationContextRegistrant.Object },
                arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeAction, arrangeInvocationContext, CancellationToken.None);

            // Assert
            Assert.Same(arrangeObj, actualObj);
        }

        [Fact]
        public async Task Should_rethrow_original_exception_When_action_throws_exception()
        {
            // Arrange 
            var mockInvocationContext = new Mock<IServiceDelegateInvocationContext>();
            var mockServices = new Mock<IServiceProvider>();

            var arrangeAction = new Action(
                () => throw new TestException());

            var arrangeInvocationContext = mockInvocationContext.Object;
            var arrangeServices = mockServices.Object;
            var arrangeInvoker = new ServiceDelegateInvoker(
                Enumerable.Empty<IServiceDelegateInvocationContextRegistrant>(),
                arrangeServices);

            // Assert
            await Assert.ThrowsAsync<TestException>(
               async () =>
               {
                   // Act
                   await arrangeInvoker.InvokeAsync(arrangeAction, arrangeInvocationContext, CancellationToken.None);
               });
        }

        [Fact]
        public async Task Should_return_original_task_When_function_returns_task()
        {
            // Arrange 
            var arrangeTaskCompletionSource = new TaskCompletionSource<int>();
            var arrangeTask = arrangeTaskCompletionSource.Task;

            var mockInvocationContext = new Mock<IServiceDelegateInvocationContext>();
            var mockServices = new Mock<IServiceProvider>();

            Task actualTask = null;

            var arrangeFunction = new Func<Task>(
                () => arrangeTask);

            var arrangeInvocationContext = mockInvocationContext.Object;
            var arrangeServices = mockServices.Object;
            var arrangeInvoker = new ServiceDelegateInvoker(
                Enumerable.Empty<IServiceDelegateInvocationContextRegistrant>(),
                arrangeServices);

            // Act
            arrangeTaskCompletionSource.SetResult(0);

            actualTask = arrangeInvoker.InvokeAsync(arrangeFunction, arrangeInvocationContext, CancellationToken.None);
            await actualTask;

            // Assert
            Assert.Same(arrangeTask, actualTask);
        }
    }
}