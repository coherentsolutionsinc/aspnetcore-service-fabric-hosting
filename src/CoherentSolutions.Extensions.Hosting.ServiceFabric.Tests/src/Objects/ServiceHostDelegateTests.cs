using System;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class ServiceHostDelegateTests
    {
        private interface IMyDependency
        {
        }

        private class MyDependency : IMyDependency
        {
        }

        private class MyException : Exception
        {
        }

        [Fact]
        public void
            Should_pass_cancellation_token_to_wrapped_delegate_When_invoking()
        {
            // Arrange 
            var cancellationTokenSource = new CancellationTokenSource();

            var expectedHash = cancellationTokenSource.Token.GetHashCode();
            var actualHash = 0;

            var services = new Mock<IServiceProvider>();
            var @delegate = new ServiceHostDelegate(
                new Action<CancellationToken>(cancellationToken => actualHash = cancellationToken.GetHashCode()),
                services.Object);

            // Act
            @delegate.InvokeAsync(cancellationTokenSource.Token).GetAwaiter().GetResult();

            // Assert
            services.VerifyNoOtherCalls();

            Assert.Equal(expectedHash, actualHash);
        }

        [Fact]
        public void
            Should_propagate_original_exception_When_invoking()
        {
            // Arrange
            var services = new Mock<IServiceProvider>();

            var @delegate = new ServiceHostDelegate(
                new Action(() => throw new MyException()),
                services.Object);

            // Act, Assert
            Assert.Throws<MyException>(
                () =>
                {
                    @delegate.InvokeAsync(CancellationToken.None).GetAwaiter().GetResult();
                });

            services.VerifyNoOtherCalls();
        }

        [Fact]
        public void
            Should_resolve_arguments_using_services_When_invoking()
        {
            // Arrange
            var root = new MyDependency();

            object expectedObject = root;
            object expectedInterface = root;
            object actualObject = null;
            object actualInterface = null;

            var services = new Mock<IServiceProvider>();
            services.Setup(instance => instance.GetService(typeof(MyDependency))).Returns(root).Verifiable();
            services.Setup(instance => instance.GetService(typeof(IMyDependency))).Returns(root).Verifiable();

            var @delegate = new ServiceHostDelegate(
                new Action<MyDependency, IMyDependency>(
                    (
                        @object,
                        @interface) =>
                    {
                        actualObject = @object;
                        actualInterface = @interface;
                    }),
                services.Object);

            // Act
            @delegate.InvokeAsync(CancellationToken.None).GetAwaiter().GetResult();

            // Assert
            services.Verify();

            Assert.Same(expectedObject, actualObject);
            Assert.Same(expectedInterface, actualInterface);
        }

        [Fact]
        public void
            Should_return_original_task_When_invoking_delegate_returining_task()
        {
            // Arrange 
            var root = new TaskCompletionSource<int>().Task;

            object expectedTask = root;
            object actualTask = null;

            var services = new Mock<IServiceProvider>();
            var @delegate = new ServiceHostDelegate(
                new Func<Task>(() => root),
                services.Object);

            // Act
            actualTask = @delegate.InvokeAsync(CancellationToken.None);

            // Assert
            services.VerifyNoOtherCalls();

            Assert.Equal(expectedTask, actualTask);
        }
    }
}