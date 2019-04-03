using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.ServiceFabric.Data;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class StatefulServiceHostDelegateInvokerTests : ServiceHostDelegateInvokerTests<IStatefulServiceDelegateInvocationContext>
    {
        protected override IStatefulServiceDelegateInvocationContext CreateInvocationContext()
        {
            return new StatefulServiceDelegateInvocationContext(StatefulServiceLifecycleEvent.OnRun);
        }

        protected override ServiceHostDelegateInvoker<IStatefulServiceDelegateInvocationContext> CreateInvokerInstance(
            Delegate @delegate,
            IServiceProvider services)
        {
            return new StatefulServiceHostDelegateInvoker(@delegate, services);
        }

        public static IEnumerable<object[]> DelegateContexts()
        {
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnCodePackageAdded(
                    new ServiceEventPayloadOnPackageAdded<CodePackage>(null))
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnCodePackageModified(
                    new ServiceEventPayloadOnPackageModified<CodePackage>(null, null))
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnCodePackageRemoved(
                    new ServiceEventPayloadOnPackageRemoved<CodePackage>(null))
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnConfigPackageAdded(
                    new ServiceEventPayloadOnPackageAdded<ConfigurationPackage>(null))
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnConfigPackageModified(
                    new ServiceEventPayloadOnPackageModified<ConfigurationPackage>(null, null))
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnConfigPackageRemoved(
                    new ServiceEventPayloadOnPackageRemoved<ConfigurationPackage>(null)),
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnDataPackageAdded(
                    new ServiceEventPayloadOnPackageAdded<DataPackage>(null))
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnDataPackageModified(
                    new ServiceEventPayloadOnPackageModified<DataPackage>(null, null))
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnDataPackageRemoved(
                    new ServiceEventPayloadOnPackageRemoved<DataPackage>(null))
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnChangeRole(
                    new StatefulServiceEventPayloadOnChangeRole(ReplicaRole.Primary))
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnDataLoss(
                    new StatefulServiceEventPayloadOnDataLoss(new StatefulServiceRestoreContext(new RestoreContext())))
            };
            yield return new object[]
            {
                new StatefulServiceDelegateInvocationContextOnShutdown(new StatefulServiceEventPayloadOnShutdown(false))
            };
        }

        [Theory]
        [MemberData(nameof(DelegateContexts))]
        public async Task Should_resolve_on_payload_argument_When_reacting_on_event<TPayload>(
            StatefulServiceDelegateInvocationContext<TPayload> context)
        {
            // Arrange 
            var expectedPayload = context.Payload;
            TPayload actualPayload = default;

            var mockServices = new Mock<IServiceProvider>();

            var arrangeInvocationContext = context;
            var arrangeServices = mockServices.Object;
            var arrangeAction = new Action<TPayload>(payload => actualPayload = payload);

            var arrangeInvoker = this.CreateInvokerInstance(arrangeAction, arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeInvocationContext, CancellationToken.None);

            // Assert
            mockServices.VerifyNoOtherCalls();

            Assert.Equal(expectedPayload, actualPayload);
        }
    }

    public class StatelessServiceHostDelegateInvokerTests : ServiceHostDelegateInvokerTests<IStatelessServiceDelegateInvocationContext>
    {
        protected override IStatelessServiceDelegateInvocationContext CreateInvocationContext()
        {
            return new StatelessServiceDelegateInvocationContext(StatelessServiceLifecycleEvent.OnRun);
        }

        protected override ServiceHostDelegateInvoker<IStatelessServiceDelegateInvocationContext> CreateInvokerInstance(
            Delegate @delegate,
            IServiceProvider services)
        {
            return new StatelessServiceHostDelegateInvoker(@delegate, services);
        }

        public static IEnumerable<object[]> DelegateContexts()
        {
            yield return new object[]
            {
                new StatelessServiceDelegateInvocationContextOnCodePackageAdded(
                    new ServiceEventPayloadOnPackageAdded<CodePackage>(null))
            };
            yield return new object[]
            {
                new StatelessServiceDelegateInvocationContextOnCodePackageModified(
                    new ServiceEventPayloadOnPackageModified<CodePackage>(null, null))
            };
            yield return new object[]
            {
                new StatelessServiceDelegateInvocationContextOnCodePackageRemoved(
                    new ServiceEventPayloadOnPackageRemoved<CodePackage>(null))
            };
            yield return new object[]
            {
                new StatelessServiceDelegateInvocationContextOnConfigPackageAdded(
                    new ServiceEventPayloadOnPackageAdded<ConfigurationPackage>(null))
            };
            yield return new object[]
            {
                new StatelessServiceDelegateInvocationContextOnConfigPackageModified(
                    new ServiceEventPayloadOnPackageModified<ConfigurationPackage>(null, null))
            };
            yield return new object[]
            {
                new StatelessServiceDelegateInvocationContextOnConfigPackageRemoved(
                    new ServiceEventPayloadOnPackageRemoved<ConfigurationPackage>(null)),
            };
            yield return new object[]
            {
                new StatelessServiceDelegateInvocationContextOnDataPackageAdded(
                    new ServiceEventPayloadOnPackageAdded<DataPackage>(null))
            };
            yield return new object[]
            {
                new StatelessServiceDelegateInvocationContextOnDataPackageModified(
                    new ServiceEventPayloadOnPackageModified<DataPackage>(null, null))
            };
            yield return new object[]
            {
                new StatelessServiceDelegateInvocationContextOnDataPackageRemoved(
                    new ServiceEventPayloadOnPackageRemoved<DataPackage>(null))
            };
            yield return new object[]
            {
                new StatelessServiceDelegateInvocationContextOnShutdown(new StatelessServiceEventPayloadOnShutdown(false))
            };
        }

        [Theory]
        [MemberData(nameof(DelegateContexts))]
        public async Task Should_resolve_on_payload_argument_When_reacting_on_event<TPayload>(
            StatelessServiceDelegateInvocationContext<TPayload> context)
        {
            // Arrange 
            var expectedPayload = context.Payload;
            TPayload actualPayload = default;

            var mockServices = new Mock<IServiceProvider>();

            var arrangeInvocationContext = context;
            var arrangeServices = mockServices.Object;
            var arrangeAction = new Action<TPayload>(payload => actualPayload = payload);

            var arrangeInvoker = this.CreateInvokerInstance(arrangeAction, arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeInvocationContext, CancellationToken.None);

            // Assert
            mockServices.VerifyNoOtherCalls();

            Assert.Equal(expectedPayload, actualPayload);
        }
    }

    public abstract class ServiceHostDelegateInvokerTests<TInvocationContext>
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

        protected abstract TInvocationContext CreateInvocationContext();

        protected abstract ServiceHostDelegateInvoker<TInvocationContext> CreateInvokerInstance(
            Delegate @delegate,
            IServiceProvider services);

        [Fact]
        public async Task Should_resolve_arguments_When_action_has_parameters()
        {
            // Arrange 
            var arrangeObject = new TestDependency();

            var expectedByType = arrangeObject;
            ITestDependency expectedByInterface = arrangeObject;
            TestDependency actualByType = null;
            ITestDependency actualByInterface = null;

            var arrangeAction = new Action<TestDependency, ITestDependency>(
                (
                    byType,
                    byInterface) =>
                {
                    actualByType = byType;
                    actualByInterface = byInterface;
                });

            var mockServices = new Mock<IServiceProvider>();
            mockServices
               .Setup(instance => instance.GetService(typeof(TestDependency)))
               .Returns(expectedByType)
               .Verifiable();

            mockServices
               .Setup(instance => instance.GetService(typeof(ITestDependency)))
               .Returns(expectedByInterface)
               .Verifiable();

            var arrangeServices = mockServices.Object;

            var arrangeInvocationContext = this.CreateInvocationContext();
            var arrangeInvoker = this.CreateInvokerInstance(arrangeAction, arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeInvocationContext, CancellationToken.None);

            // Assert
            mockServices.Verify();

            Assert.Same(expectedByType, actualByType);
            Assert.Same(expectedByInterface, actualByInterface);
        }

        [Fact]
        public async Task Should_resolve_cancellation_token_argument_When_action_has_cancellation_token_parameter()
        {
            // Arrange 
            var arrangeCancellationTokenSource = new CancellationTokenSource();
            var arrangeCancellationToken = arrangeCancellationTokenSource.Token;

            var mockServices = new Mock<IServiceProvider>();

            var expectedCancellationToken = arrangeCancellationTokenSource.Token;
            CancellationToken actualCancellationToken = default;

            var arrangeServices = mockServices.Object;
            var arrangeAction = new Action<CancellationToken>(cancellationToken => actualCancellationToken = cancellationToken);

            var arrangeInvocationContext = this.CreateInvocationContext();
            var arrangeInvoker = this.CreateInvokerInstance(arrangeAction, arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeInvocationContext, arrangeCancellationToken);

            // Assert
            mockServices.VerifyNoOtherCalls();

            Assert.Equal(expectedCancellationToken.GetHashCode(), actualCancellationToken.GetHashCode());
        }

        [Fact]
        public async Task Should_resolve_invocation_context_argument_When_action_has_invocation_context_parameter()
        {
            // Arrange 
            var arrangeInvocationContext = this.CreateInvocationContext();

            var mockServices = new Mock<IServiceProvider>();

            var expectedInvocationContext = arrangeInvocationContext;
            TInvocationContext actualInvocationContext = default;

            var arrangeServices = mockServices.Object;
            var arrangeAction = new Action<TInvocationContext>(invocationContext => actualInvocationContext = invocationContext);

            var arrangeInvoker = this.CreateInvokerInstance(arrangeAction, arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeInvocationContext, CancellationToken.None);

            // Assert
            mockServices.VerifyNoOtherCalls();

            Assert.Equal(expectedInvocationContext, actualInvocationContext);
        }

        [Fact]
        public async Task Should_rethrow_original_exception_When_action_throws_exception()
        {
            // Arrange 
            var mockServices = new Mock<IServiceProvider>();

            var arrangeServices = mockServices.Object;
            var arrangeAction = new Action<CancellationToken>(cancellationToken => throw new TestException());

            var arrangeInvocationContext = this.CreateInvocationContext();
            var arrangeInvoker = this.CreateInvokerInstance(arrangeAction, arrangeServices);

            // Assert
            await Assert.ThrowsAsync<TestException>(
                async () =>
                {
                    // Act
                    await arrangeInvoker.InvokeAsync(arrangeInvocationContext, CancellationToken.None);
                });

            mockServices.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Should_return_original_task_When_function_returns_task()
        {
            // Arrange 
            var arrangeTaskCompletionSource = new TaskCompletionSource<int>();
            var arrangeTask = arrangeTaskCompletionSource.Task;

            var mockServices = new Mock<IServiceProvider>();

            Task expectedTask = arrangeTask;
            Task actualTask = null;

            var arrangeServices = mockServices.Object;
            var arrangeFunction = new Func<Task>(() => arrangeTask);

            var arrangeInvocationContext = this.CreateInvocationContext();
            var arrangeInvoker = this.CreateInvokerInstance(arrangeFunction, arrangeServices);

            // Act
            arrangeTaskCompletionSource.SetResult(0);

            actualTask = arrangeInvoker.InvokeAsync(arrangeInvocationContext, CancellationToken.None);
            await actualTask;

            // Assert
            mockServices.Verify();
            mockServices.VerifyNoOtherCalls();

            Assert.Same(expectedTask, actualTask);
        }
    }
}