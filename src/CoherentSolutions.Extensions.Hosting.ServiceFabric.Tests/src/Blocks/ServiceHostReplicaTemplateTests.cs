using System;
using System.Collections.Generic;
using System.Text;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.src.Blocks
{
    public class ServiceHostReplicaGenericStub
    {

    }

    public abstract class ServiceHostReplicaTemplateTests<TInput, TOutput, TParameters, TConfigurator>
        where TParameters : IServiceHostReplicaTemplateParameters
        where TConfigurator : IServiceHostReplicaTemplateConfigurator
    {
        protected abstract ServiceHostReplicaTemplate<TInput, TOutput, TParameters, TConfigurator> CreateReplicaTemplate();

        [Fact]
        public void AA()
        {
            // Mock
            var mockServiceCollection = new Mock<IServiceCollection>();
            mockServiceCollection
               .Setup(instance => instance.Add(It.Is<ServiceDescriptor>(descriptor => descriptor.ServiceType == typeof(ITestDependency))))
               .Verifiable();

            // Arrange
            var arrangeServiceCollection = mockServiceCollection.Object;
            var arrangeReplicaTemplate = this.CreateReplicaTemplate();

            // Act
            arrangeReplicaTemplate.ConfigureObject(
                configurator =>
                {
                    configurator.UseDependencies(() => arrangeServiceCollection);
                    configurator.ConfigureDependencies(
                        dependencies =>
                        {
                            dependencies.Add(
                                new ServiceDescriptor(typeof(ITestDependency), typeof(TestDependency), ServiceLifetime.Transient));
                        });
                });
            //arrangeReplicaTemplate.Activate();

            // Assert
            mockServiceCollection.Verify();
        }
    }
}
