using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.DependencyInjection.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using ServiceFabric.Mocks;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private interface IMyServiceEventSourceInterface : IServiceEventSourceInterface
        {
        }

        private class ServiceEventSource : IServiceEventSource, IMyServiceEventSourceInterface
        {
            public void WriteEvent<T>(
                ref T eventData)
                where T : ServiceEventSourceData
            {
                throw new NotImplementedException();
            }
        }

        private class AspNetCoreServiceHostListenerInformation : IServiceHostAspNetCoreListenerInformation
        {
            public string EndpointName { get; }

            public string UrlSuffix { get; }
        }

        private class RemotingServiceHostListenerInformation : IServiceHostRemotingListenerInformation
        {
            public string EndpointName { get; }
        }

        [Fact]
        public void Should_register_IServiceEventSource_as_Type_and_IServiceEventSource_and_all_IServiceEventSourceInterfaces_When_Adding_IServiceEventSource()
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IServiceEventSource))))
               .Verifiable();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IServiceEventSource))))
               .Verifiable();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IMyServiceEventSourceInterface))))
               .Verifiable();

            var arrangeServiceCollection = mockServiceCollection.Object;

            // Act
            ServiceCollectionExtensions.Add(arrangeServiceCollection, new ServiceEventSource());

            // Assert
            mockServiceCollection.Verify();
        }

        [Fact]
        public void
            Should_register_IServiceHostListenerInformation_as_IServiceHostListenerInformation_and_IServiceHostAspNetCoreListenerInformation_When_Adding_IServiceHostAspNetCoreListenerInformation()
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IServiceHostListenerInformation))))
               .Verifiable();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IServiceHostAspNetCoreListenerInformation))))
               .Verifiable();

            var arrangeServiceCollection = mockServiceCollection.Object;

            // Act
            ServiceCollectionExtensions.Add(arrangeServiceCollection, new AspNetCoreServiceHostListenerInformation());

            // Assert
            mockServiceCollection.Verify();
        }

        [Fact]
        public void
            Should_register_IServiceHostListenerInformation_as_IServiceHostListenerInformation_and_IServiceHostRemotingListenerInformation_When_Adding_IServiceHostRemotingListenerInformation()
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IServiceHostListenerInformation))))
               .Verifiable();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IServiceHostRemotingListenerInformation))))
               .Verifiable();

            var arrangeServiceCollection = mockServiceCollection.Object;

            // Act
            ServiceCollectionExtensions.Add(arrangeServiceCollection, new RemotingServiceHostListenerInformation());

            // Assert
            mockServiceCollection.Verify();
        }

        [Fact]
        public void Should_register_IServicePartition_as_IServicePartition_and_IStatefulServicePartition_When_Adding_IStatefulServicePartition()
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IServicePartition))))
               .Verifiable();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IStatefulServicePartition))))
               .Verifiable();

            var arrangeServiceCollection = mockServiceCollection.Object;

            // Act
            ServiceCollectionExtensions.Add(arrangeServiceCollection, new MockStatefulServicePartition());

            // Assert
            mockServiceCollection.Verify();
        }

        [Fact]
        public void Should_register_IServicePartition_as_IServicePartition_and_IStatelessServicePartition_When_Adding_IStatelessServicePartition()
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IServicePartition))))
               .Verifiable();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(IStatelessServicePartition))))
               .Verifiable();

            var arrangeServiceCollection = mockServiceCollection.Object;

            // Act
            ServiceCollectionExtensions.Add(arrangeServiceCollection, new MockStatelessServicePartition());

            // Assert
            mockServiceCollection.Verify();
        }

        [Fact]
        public void Should_register_ServiceContext_as_ServiceContext_and_StatefulServiceContext_When_Adding_StatefulServiceContext()
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(ServiceContext))))
               .Verifiable();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(StatefulServiceContext))))
               .Verifiable();

            var arrangeServiceCollection = mockServiceCollection.Object;

            // Act
            ServiceCollectionExtensions.Add(arrangeServiceCollection, MockStatefulServiceContextFactory.Default);

            // Assert
            mockServiceCollection.Verify();
        }

        [Fact]
        public void Should_register_ServiceContext_as_ServiceContext_and_StatelessServiceContext_When_Adding_StatelessServiceContext()
        {
            // Arrange
            var mockServiceCollection = new Mock<IServiceCollection>();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(ServiceContext))))
               .Verifiable();
            mockServiceCollection
               .Setup(
                    instance => instance.Add(
                        It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(StatelessServiceContext))))
               .Verifiable();

            var arrangeServiceCollection = mockServiceCollection.Object;

            // Act
            ServiceCollectionExtensions.Add(arrangeServiceCollection, MockStatelessServiceContextFactory.Default);

            // Assert
            mockServiceCollection.Verify();
        }
    }
}