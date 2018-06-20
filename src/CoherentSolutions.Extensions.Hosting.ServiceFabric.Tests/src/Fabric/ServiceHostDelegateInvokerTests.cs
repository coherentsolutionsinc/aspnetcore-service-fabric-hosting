using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric
{
    public class ServiceHostDelegateInvokerTests
    {
        [Fact]
        public void
            Should_throw_Invoke_all_delegates_When_invoking_delegates()
        {
            // Arrange 
            var delegateOne = new Mock<IServiceHostDelegate>();
            delegateOne
               .Setup(instance => instance.InvokeAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            var delegateTwo = new Mock<IServiceHostDelegate>();
            delegateTwo
               .Setup(instance => instance.InvokeAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            var delegates = new[] { delegateOne.Object, delegateTwo.Object };

            // Act
            new ServiceHostDelegateInvoker().InvokeAsync(delegates, CancellationToken.None).GetAwaiter().GetResult();

            // Assert
            delegateOne.Verify(instance => instance.InvokeAsync(It.IsAny<CancellationToken>()), Times.Once);
            delegateTwo.Verify(instance => instance.InvokeAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}