using System;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

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
        private async Task Should_resolve_arguments_When_action_has_parameters()
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
            mockServices.VerifyNoOtherCalls();

            Assert.Same(expectedByType, actualByType);
            Assert.Same(expectedByInterface, actualByInterface);
        }

        [Fact]
        private async Task Should_resolve_invocation_context_argument_When_action_has_invocation_context_parameter()
        {
            // Arrange 
            var arrangeInvocationContext = this.CreateInvocationContext();

            var mockServices = new Mock<IServiceProvider>();
            mockServices
               .Setup(instance => instance.GetService(typeof(TInvocationContext)))
               .Returns(arrangeInvocationContext)
               .Verifiable();

            TInvocationContext expectedInvocationContext = arrangeInvocationContext;
            TInvocationContext actualInvocationContext = default;

            var arrangeServices = mockServices.Object;
            var arrangeAction = new Action<TInvocationContext>(invocationContext => actualInvocationContext = invocationContext);

            var arrangeInvoker = this.CreateInvokerInstance(arrangeAction, arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeInvocationContext, CancellationToken.None);

            // Assert
            mockServices.Verify();
            mockServices.VerifyNoOtherCalls();

            Assert.Equal(expectedInvocationContext, actualInvocationContext);
        }

        [Fact]
        private async Task Should_resolve_cancellation_token_argument_When_action_has_cancellation_token_parameter()
        {
            // Arrange 
            var arrangeCancellationTokenSource = new CancellationTokenSource();
            var arrangeCancellationToken = arrangeCancellationTokenSource.Token;

            var mockServices = new Mock<IServiceProvider>();
            mockServices
               .Setup(instance => instance.GetService(typeof(CancellationToken)))
               .Returns(arrangeCancellationToken)
               .Verifiable();

            var expectedCancellationToken = arrangeCancellationTokenSource.Token;
            CancellationToken actualCancellationToken = default;

            var arrangeServices = mockServices.Object;
            var arrangeAction = new Action<CancellationToken>(cancellationToken => actualCancellationToken = cancellationToken);

            var arrangeInvocationContext = this.CreateInvocationContext();
            var arrangeInvoker = this.CreateInvokerInstance(arrangeAction, arrangeServices);

            // Act
            await arrangeInvoker.InvokeAsync(arrangeInvocationContext, expectedCancellationToken);

            // Assert
            mockServices.Verify();
            mockServices.VerifyNoOtherCalls();

            Assert.Equal(expectedCancellationToken.GetHashCode(), actualCancellationToken.GetHashCode());
        }

        [Fact]
        private async Task Should_rethrow_original_exception_When_action_throws_exception()
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
        private async Task Should_return_original_task_When_function_returns_task()
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
            actualTask = arrangeInvoker.InvokeAsync(arrangeInvocationContext, CancellationToken.None);
            await actualTask;

            // Assert
            mockServices.Verify();
            mockServices.VerifyNoOtherCalls();

            Assert.Same(expectedTask, actualTask);
        }
    }
}