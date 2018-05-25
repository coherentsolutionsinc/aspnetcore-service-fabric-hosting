using System;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Tools
{
    public class ConfigurableObjectTests
    {
        private class ConfigurableObjectStub : ConfigurableObject<object>
        {
            public void BuildConfiguration()
            {
                this.UpstreamConfiguration(new object());
            }
        }

        [Fact]
        public void
            Should_call_configuration_actions_in_sequence_When_UpstreamConfiguration_is_called()
        {
            // Arrange
            var sequence = new MockSequence();

            var first = new Mock<Action<object>>();
            first
               .InSequence(sequence)
               .Setup(instance => instance(It.IsAny<object>()));

            var second = new Mock<Action<object>>();
            second
               .InSequence(sequence)
               .Setup(instance => instance(It.IsAny<object>()));

            var third = new Mock<Action<object>>();
            third
               .InSequence(sequence)
               .Setup(instance => instance(It.IsAny<object>()));

            // Act
            var obj = new ConfigurableObjectStub();
            obj.ConfigureObject(first.Object);
            obj.ConfigureObject(second.Object);
            obj.ConfigureObject(third.Object);

            obj.BuildConfiguration();

            // Assert
            first.Verify(instance => instance(It.IsAny<object>()), Times.Once());

            second.Verify(instance => instance(It.IsAny<object>()), Times.Once());

            third.Verify(instance => instance(It.IsAny<object>()), Times.Once());
        }
    }
}