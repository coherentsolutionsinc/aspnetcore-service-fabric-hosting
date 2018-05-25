using System;
using System.Fabric;
using System.Threading;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Fabric.Services
{
    public class StatelessServiceHostAspNetCoreListenerReplicaTemplateTests
        : ServiceHostAspNetCoreListenerReplicaTemplateTests<
            IStatelessService,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplateParameters,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
            ServiceInstanceListener>
    {
        private class StatelessInvoker : Invoker
        {
            public StatelessInvoker(
                ServiceInstanceListener listener)
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

            private static StatelessServiceContext CreateContext()
            {
                return new StatelessServiceContext(
                    new NodeContext("default-node", new NodeId(0, int.MaxValue), 0, "default-node-type", "127.0.0.1"),
                    new Mock<ICodePackageActivationContext>().Object,
                    "",
                    new Uri("fabric:/stateless-service", UriKind.Absolute),
                    null,
                    Guid.Empty,
                    0);
            }
        }

        protected override IStatelessService CreateService()
        {
            return new StatelessServiceStub();
        }

        protected override ServiceHostAspNetCoreListenerReplicaTemplate<IStatelessService, IStatelessServiceHostAspNetCoreListenerReplicaTemplateParameters,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator, ServiceInstanceListener> CreateInstance()
        {
            return new StatelessServiceHostAspNetCoreListenerReplicaTemplate();
        }

        protected override Invoker CreateInvoker(
            ServiceInstanceListener listener)
        {
            return new StatelessInvoker(listener);
        }

        [Fact]
        public void
            Should_configure_listener_name_When_configured_endpoint_name()
        {
            // Arrange
            var service = new Mock<IStatelessService>();

            var endpoint = "endpoint";

            // Act
            var listener = new StatelessServiceHostAspNetCoreListenerReplicaTemplate()
               .UseAspNetCoreCommunicationListener(AspNetCoreCommunicationListenerStub.Func)
               .UseWebHostBuilder(WebHostBuilderStub.Func)
               .UseEndpointName(endpoint)
               .Activate(service.Object);

            // Assert
            Assert.Equal(endpoint, listener.Name);
        }

        [Fact]
        public void
            Should_configure_stateless_service_context_as_singleton_service_When_activating_replica_template()
        {
            // Arrange
            var service = this.CreateService();

            var serviceCollection = new Mock<IServiceCollection>();

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
                    config.UseAspNetCoreCommunicationListener(AspNetCoreCommunicationListenerStub.Func);
                    config.UseWebHostBuilder(() => builder.Object);
                    config.UseWebHostBuilderExtensionsImpl(WebHostBuilderExtensionsImplStub.Func);
                });

            var listener = replicaTemplate.Activate(service);

            var invoker = this.CreateInvoker(listener);
            invoker.Invoke();

            // Assert
            serviceCollection.Verify(
                instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(StatelessServiceContext) == v.ServiceType)),
                Times.Once());
        }

        [Fact]
        public void
            Should_configure_stateless_service_partition_as_singleton_When_activating_replica_template()
        {
            // Arrange
            var service = this.CreateService();

            var serviceCollection = new Mock<IServiceCollection>();

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
                    config.UseAspNetCoreCommunicationListener(AspNetCoreCommunicationListenerStub.Func);
                    config.UseWebHostBuilder(() => builder.Object);
                });

            var listener = replicaTemplate.Activate(service);

            var invoker = this.CreateInvoker(listener);
            invoker.Invoke();

            // Assert
            serviceCollection.Verify(
                instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(IStatelessServicePartition) == v.ServiceType)),
                Times.Once());
        }
    }
}