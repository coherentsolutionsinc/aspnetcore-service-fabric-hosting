using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Stubs;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

using Moq;

using ServiceFabric.Mocks;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric.Services
{
    public class StatefulServiceHostAspNetCoreListenerReplicaTemplateTests
        : ServiceHostAspNetCoreListenerReplicaTemplateTests<
            IStatefulService,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplateParameters,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
            ServiceReplicaListener>
    {
        private class StatefulInvoker : Invoker
        {
            public StatefulInvoker(
                ServiceReplicaListener listener)
                : base(listener)
            {
            }

            public override void Invoke()
            {
                this.Listener.CreateCommunicationListener(CreateContext())
                   .OpenAsync(CancellationToken.None)
                   .GetAwaiter()
                   .GetResult();
            }

            private static StatefulServiceContext CreateContext()
            {
                return MockStatefulServiceContextFactory.Default;
            }
        }

        protected override IStatefulService CreateService()
        {
            var setup = new Mock<IStatefulService>();

            setup.Setup(instance => instance.GetContext()).Returns(MockStatefulServiceContextFactory.Default);
            setup.Setup(instance => instance.GetPartition()).Returns(new Mock<IStatefulServicePartition>().Object);
            setup.Setup(instance => instance.GetEventSource()).Returns(new Mock<IServiceEventSource>().Object);
            setup.Setup(instance => instance.GetReliableStateManager()).Returns(new Mock<IReliableStateManager>().Object);

            return setup.Object;
        }

        protected override ServiceHostAspNetCoreListenerReplicaTemplate<IStatefulService, IStatefulServiceHostAspNetCoreListenerReplicaTemplateParameters,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator, ServiceReplicaListener> CreateInstance()
        {
            return new StatefulServiceHostAspNetCoreListenerReplicaTemplate();
        }

        protected override Invoker CreateInvoker(
            ServiceReplicaListener listener)
        {
            return new StatefulInvoker(listener);
        }

        [Fact]
        public void
            Should_configure_listener_name_When_configured_endpoint_name()
        {
            // Arrange
            var service = new Mock<IStatefulService>();

            var endpoint = "endpoint";

            // Act
            var listener = new StatefulServiceHostAspNetCoreListenerReplicaTemplate()
               .UseCommunicationListener(AspNetCoreCommunicationListenerStub.Func)
               .UseWebHostBuilder(WebHostBuilderStub.Func)
               .UseEndpointName(endpoint)
               .Activate(service.Object);

            // Assert
            Assert.Equal(endpoint, listener.Name);
        }

        [Fact]
        public void
            Should_configure_listener_to_listen_on_secondary_When_configured_to_listen_on_secondary()
        {
            // Arrange
            var service = new Mock<IStatefulService>();

            // Act
            var listener = new StatefulServiceHostAspNetCoreListenerReplicaTemplate()
               .UseCommunicationListener(AspNetCoreCommunicationListenerStub.Func)
               .UseWebHostBuilder(WebHostBuilderStub.Func)
               .UseListenerOnSecondary()
               .Activate(service.Object);

            // Assert
            Assert.True(listener.ListenOnSecondary);
        }

        [Fact]
        public void
            Should_configure_reliable_state_manager_as_singleton_When_activating_replica_template()
        {
            // Arrange
            var service = this.CreateService();

            var serviceCollection = new Mock<IServiceCollection>();
            serviceCollection
               .Setup(instance => instance.GetEnumerator())
               .Returns(new Mock<IEnumerator<ServiceDescriptor>>().Object);

            var builder = new Mock<IWebHostBuilder>(MockBehavior.Loose);
            builder
               .Setup(instance => instance.Build())
               .Returns(new Mock<IWebHost>().Object);
            builder
               .Setup(instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
               .Callback<Action<IServiceCollection>>(action => action(serviceCollection.Object))
               .Returns(builder.Object);

            // Act
            var replicaTemplate = new StatefulServiceHostAspNetCoreListenerReplicaTemplate()
               .UseCommunicationListener(AspNetCoreCommunicationListenerStub.Func)
               .UseWebHostBuilder(() => builder.Object);

            var listener = replicaTemplate.Activate(service);

            var invoker = new StatefulInvoker(listener);
            invoker.Invoke();

            // Assert
            serviceCollection.Verify(
                instance => instance.Add(It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IReliableStateManager))),
                Times.Once());
        }

        [Fact]
        public void
            Should_configure_stateful_service_context_as_singleton_service_When_activating_replica_template()
        {
            // Arrange
            var service = this.CreateService();

            var serviceCollection = new Mock<IServiceCollection>();
            serviceCollection
               .Setup(instance => instance.GetEnumerator())
               .Returns(new Mock<IEnumerator<ServiceDescriptor>>().Object);

            var builder = new Mock<IWebHostBuilder>(MockBehavior.Loose);
            builder
               .Setup(instance => instance.Build())
               .Returns(new Mock<IWebHost>().Object);
            builder
               .Setup(instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
               .Callback<Action<IServiceCollection>>(action => action(serviceCollection.Object))
               .Returns(builder.Object);

            // Act
            var replicaTemplate = this.CreateInstance();
            replicaTemplate.ConfigureObject(
                config =>
                {
                    config.UseCommunicationListener(AspNetCoreCommunicationListenerStub.Func);
                    config.UseWebHostBuilder(() => builder.Object);
                    config.UseWebHostBuilderExtensionsImpl(WebHostBuilderExtensionsImplStub.Func);
                });

            var listener = replicaTemplate.Activate(service);

            var invoker = this.CreateInvoker(listener);
            invoker.Invoke();

            // Assert
            serviceCollection.Verify(
                instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(StatefulServiceContext) == v.ServiceType)),
                Times.Once());
        }

        [Fact]
        public void
            Should_configure_stateful_service_partition_as_singleton_When_activating_replica_template()
        {
            // Arrange
            var service = this.CreateService();

            var serviceCollection = new Mock<IServiceCollection>();
            serviceCollection
               .Setup(instance => instance.GetEnumerator())
               .Returns(new Mock<IEnumerator<ServiceDescriptor>>().Object);

            var builder = new Mock<IWebHostBuilder>(MockBehavior.Loose);
            builder
               .Setup(instance => instance.Build())
               .Returns(new Mock<IWebHost>().Object);
            builder
               .Setup(instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
               .Callback<Action<IServiceCollection>>(action => action(serviceCollection.Object))
               .Returns(builder.Object);

            // Act
            var replicaTemplate = this.CreateInstance();
            replicaTemplate.ConfigureObject(
                config =>
                {
                    config.UseCommunicationListener(AspNetCoreCommunicationListenerStub.Func);
                    config.UseWebHostBuilder(() => builder.Object);
                });

            var listener = replicaTemplate.Activate(service);

            var invoker = this.CreateInvoker(listener);
            invoker.Invoke();

            // Assert
            serviceCollection.Verify(
                instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(IStatefulServicePartition) == v.ServiceType)),
                Times.Once());
        }
    }
}