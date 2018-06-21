using System;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric
{
    public class ServiceHostDelegateTests
    {
        [Fact]
        public void
            Should_throw_ArgumentException_When_delegate_return_type_isnt_Task()
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new ServiceHostDelegate(
                new Func<int>(() => 0),
                ServiceLifecycleEvent.Unknown,
                new Mock<IServiceProvider>().Object));
        }

        [Fact]
        public void
            Should_resolve_arguments_using_service_provider_When_delegate_is_invoked()
        {
            // Arrange
            var originalValue = new ArgumentException();

            var provider = new Mock<IServiceProvider>();
            provider
               .Setup(instance => instance.GetService(typeof(ArgumentException)))
               .Returns(originalValue);

            // Act
            ArgumentException expectedValue = null;
            var @delegate = new ServiceHostDelegate(
                new Func<ArgumentException, Task>(
                    value =>
                    {
                        expectedValue = value;
                        return Task.CompletedTask;
                    }),
                ServiceLifecycleEvent.Unknown,
                provider.Object);

            @delegate.InvokeAsync(CancellationToken.None).GetAwaiter().GetResult();

            // Assert
            Assert.Same(originalValue, expectedValue);
        }

        [Fact]
        public void
            Should_resolve_cancellationtoken_and_cancel_delegate_When_InvokeAsync_cancellationtoken_canceled()
        {
            // Arrange
            var provider = new Mock<IServiceProvider>();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act
            var @delegate = new ServiceHostDelegate(
                new Func<CancellationToken, Task>(
                    value =>
                    {
                        value.ThrowIfCancellationRequested();

                        return Task.CompletedTask;
                    }),
                ServiceLifecycleEvent.Unknown,
                provider.Object);

            // Assert
            Assert.Throws<OperationCanceledException>(
                () =>
                {
                    @delegate.InvokeAsync(cancellationTokenSource.Token).GetAwaiter().GetResult();
                });
        }
    }
}
