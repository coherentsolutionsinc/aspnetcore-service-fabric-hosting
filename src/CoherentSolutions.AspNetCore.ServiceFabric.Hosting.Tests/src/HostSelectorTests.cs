using System;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests
{
    public class HostSelectorTests
    {
        [Fact]
        public void
            Should_return_matching_host_descriptor_When_providers_keywords_match_host_keywords()
        {
            // Arrange
            var keywordsProvider = new Mock<IHostKeywordsProvider>();
            keywordsProvider
               .Setup(instance => instance.GetKeywords())
               .Returns(new[] { "custom-keyword" });

            var hostKeywords = new Mock<IHostKeywords>();
            hostKeywords
               .Setup(instance => instance.GetKeywords())
               .Returns(new[] { "custom-keyword" });

            var hostRunner = new Mock<IHostRunner>();

            var hostDescriptor = new Mock<IHostDescriptor>();
            hostDescriptor
               .Setup(instance => instance.Keywords)
               .Returns(hostKeywords.Object);
            hostDescriptor
               .Setup(instance => instance.Runner)
               .Returns(hostRunner.Object);

            // Act
            var hostSelector = new HostSelector();

            var resultDescriptor = hostSelector.Select(
                new[] { keywordsProvider.Object },
                new[] { hostDescriptor.Object });

            // Assert
            Assert.Same(hostDescriptor.Object, resultDescriptor);
        }

        [Fact]
        public void
            Should_return_matching_host_descriptor_When_providers_keywords_match_multiple_hosts_keywords_with_different_relativity()
        {
            // Arrange
            var keywordsProvider = new Mock<IHostKeywordsProvider>();
            keywordsProvider
               .Setup(instance => instance.GetKeywords())
               .Returns(new[] { "custom-keyword", "more-custom-keyword" });

            var hostKeywordsLessRelative = new Mock<IHostKeywords>();
            hostKeywordsLessRelative
               .Setup(instance => instance.GetKeywords())
               .Returns(new[] { "custom-keyword" });

            var hostKeywordsMoreRelative = new Mock<IHostKeywords>();
            hostKeywordsMoreRelative
               .Setup(instance => instance.GetKeywords())
               .Returns(new[] { "custom-keyword", "more-custom-keyword" });

            var hostRunner = new Mock<IHostRunner>();

            var hostDescriptorLessRelative = new Mock<IHostDescriptor>();
            hostDescriptorLessRelative
               .Setup(instance => instance.Keywords)
               .Returns(hostKeywordsLessRelative.Object);
            hostDescriptorLessRelative
               .Setup(instance => instance.Runner)
               .Returns(hostRunner.Object);

            var hostDescriptorMoreRelative = new Mock<IHostDescriptor>();
            hostDescriptorMoreRelative
               .Setup(instance => instance.Keywords)
               .Returns(hostKeywordsMoreRelative.Object);
            hostDescriptorMoreRelative
               .Setup(instance => instance.Runner)
               .Returns(hostRunner.Object);

            // Act
            var hostSelector = new HostSelector();

            var resultDescriptor = hostSelector.Select(
                new[] { keywordsProvider.Object },
                new[]
                {
                    hostDescriptorLessRelative.Object,
                    hostDescriptorMoreRelative.Object
                });

            // Assert
            Assert.Same(hostDescriptorMoreRelative.Object, resultDescriptor);
        }

        [Fact]
        public void
            Should_throw_InvalidOperationException_When_providers_keywords_do_match_host_keywords()
        {
            // Arrange
            var keywordsProvider = new Mock<IHostKeywordsProvider>();
            keywordsProvider
               .Setup(instance => instance.GetKeywords())
               .Returns(new[] { "unique-keyword" });

            var hostKeywords = new Mock<IHostKeywords>();
            hostKeywords
               .Setup(instance => instance.GetKeywords())
               .Returns(new[] { "custom-keyword" });

            var hostRunner = new Mock<IHostRunner>();

            var hostDescriptor = new Mock<IHostDescriptor>();
            hostDescriptor
               .Setup(instance => instance.Keywords)
               .Returns(hostKeywords.Object);
            hostDescriptor
               .Setup(instance => instance.Runner)
               .Returns(hostRunner.Object);

            // Act
            var hostSelector = new HostSelector();

            // Assert
            Assert.Throws<InvalidOperationException>(
                () => hostSelector.Select(new[] { keywordsProvider.Object }, new[] { hostDescriptor.Object }));
        }

        [Fact]
        public void
            Should_throw_InvalidOperationException_When_providers_keywords_match_multiple_hosts_keywords_with_same_relativity()
        {
            // Arrange
            var keywordsProvider = new Mock<IHostKeywordsProvider>();
            keywordsProvider
               .Setup(instance => instance.GetKeywords())
               .Returns(new[] { "custom-keyword" });

            var hostKeywordsFirst = new Mock<IHostKeywords>();
            hostKeywordsFirst
               .Setup(instance => instance.GetKeywords())
               .Returns(new[] { "custom-keyword" });

            var hostKeywordsSecond = new Mock<IHostKeywords>();
            hostKeywordsSecond
               .Setup(instance => instance.GetKeywords())
               .Returns(new[] { "custom-keyword" });

            var hostRunner = new Mock<IHostRunner>();

            var hostDescriptorFirst = new Mock<IHostDescriptor>();
            hostDescriptorFirst
               .Setup(instance => instance.Keywords)
               .Returns(hostKeywordsFirst.Object);
            hostDescriptorFirst
               .Setup(instance => instance.Runner)
               .Returns(hostRunner.Object);

            var hostDescriptorSecond = new Mock<IHostDescriptor>();
            hostDescriptorSecond
               .Setup(instance => instance.Keywords)
               .Returns(hostKeywordsSecond.Object);
            hostDescriptorSecond
               .Setup(instance => instance.Runner)
               .Returns(hostRunner.Object);

            // Act
            var hostSelector = new HostSelector();

            // Assert
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    hostSelector.Select(
                        new[] { keywordsProvider.Object },
                        new[]
                        {
                            hostDescriptorFirst.Object,
                            hostDescriptorSecond.Object
                        });
                });
        }
    }
}