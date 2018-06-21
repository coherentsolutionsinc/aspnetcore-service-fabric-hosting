using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects.Base
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

            var serviceCollection = new Mock<ServiceCollection>
            {
                CallBase = true
            };
            serviceCollection
               .As<IServiceCollection>()
               .Setup(instance => instance.Add(It.IsAny<ServiceDescriptor>()));

            // Act
            var replicaTemplate = this.CreateInstance();
            replicaTemplate.ConfigureObject(
                config =>
                {
                    config.UseDependencies(() => serviceCollection.Object);
                    config.UseDelegate(
                        new Action(
                            () =>
                            {
                            }));
                });

            replicaTemplate.Activate(service);

            // Assert
            serviceCollection
               .As<IServiceCollection>()
               .Verify(instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(ServiceContext) == v.ServiceType)), Times.Once());
            serviceCollection
               .As<IServiceCollection>()
               .Verify(instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(IServicePartition) == v.ServiceType)), Times.Once());
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
            return Tools.StatefulService;
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