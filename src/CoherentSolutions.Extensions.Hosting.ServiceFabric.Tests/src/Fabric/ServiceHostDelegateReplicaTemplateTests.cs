using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Data;

using Moq;

using ServiceFabric.Mocks;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric
{
    public abstract class ServiceHostDelegateReplicaTemplateTests<TService, TParameters, TConfigurator, TDelegate>
        where TService : IService
        where TParameters : IServiceHostDelegateReplicaTemplateParameters
        where TConfigurator : IServiceHostDelegateReplicaTemplateConfigurator
    {
        protected abstract TService CreateService();

        protected abstract ServiceHostDelegateReplicaTemplate<TService, TParameters, TConfigurator, TDelegate> CreateInstance();

        [Fact]
        public void
            Should_configure_services_When_activating_replica_template()
        {
            // Arrange
            var service = this.CreateService();

            var serviceCollection = new Mock<IServiceCollection>();
            serviceCollection
               .Setup(instance => instance.GetEnumerator())
               .Returns(new Mock<IEnumerator<ServiceDescriptor>>().Object);

            // Act
            var replicaTemplate = this.CreateInstance();
            replicaTemplate.ConfigureObject(
                config =>
                {
                    config.UseDependencies(() => serviceCollection.Object);
                    config.UseDelegate(new Func<Task>(() => Task.CompletedTask));
                });

            replicaTemplate.Activate(service);

            // Assert
            serviceCollection.Verify(
                instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(ServiceContext) == v.ServiceType)),
                Times.Once());
            serviceCollection.Verify(
                instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(IServicePartition) == v.ServiceType)),
                Times.Once());
        }
    }

    public class StatefulServiceHostDelegateReplicaTemplateTests
        : ServiceHostDelegateReplicaTemplateTests<
            IStatefulService,
            IStatefulServiceHostDelegateReplicaTemplateParameters,
            IStatefulServiceHostDelegateReplicaTemplateConfigurator,
            IServiceHostDelegate>
    {
        protected override IStatefulService CreateService()
        {
            var setup = new Mock<IStatefulService>();

            setup.Setup(instance => instance.GetContext()).Returns(MockStatefulServiceContextFactory.Default);
            setup.Setup(instance => instance.GetPartition()).Returns(new Mock<IStatefulServicePartition>().Object);
            setup.Setup(instance => instance.GetEventSource()).Returns(new Mock<IServiceEventSource>().Object);
            setup.Setup(instance => instance.GetReliableStateManager()).Returns(new Mock<IReliableStateManager>().Object);

            return setup.Object;
        }

        protected override ServiceHostDelegateReplicaTemplate<
                IStatefulService,
                IStatefulServiceHostDelegateReplicaTemplateParameters,
                IStatefulServiceHostDelegateReplicaTemplateConfigurator,
                IServiceHostDelegate>
            CreateInstance()
        {
            return new StatefulServiceHostDelegateReplicaTemplate();
        }
    }
}