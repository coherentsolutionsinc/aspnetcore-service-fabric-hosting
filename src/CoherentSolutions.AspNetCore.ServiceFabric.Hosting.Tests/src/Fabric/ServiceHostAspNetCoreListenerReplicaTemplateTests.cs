using System;
using System.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common.Exceptions;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Fabric
{
    public abstract class ServiceHostAspNetCoreListenerReplicaTemplateTests<TService, TParameters, TConfigurator, TListener>
        where TService : IService
        where TParameters : IServiceHostAspNetCoreListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
        protected abstract class Invoker
        {
            public TListener Listener { get; }

            protected Invoker(
                TListener listener)
            {
                this.Listener = listener;
            }

            public abstract void Invoke();
        }

        protected abstract TService CreateService();

        protected abstract ServiceHostAspNetCoreListenerReplicaTemplate<TService,
            TParameters,
            TConfigurator,
            TListener
        > CreateInstance();

        protected abstract Invoker CreateInvoker(
            TListener listener);

        [Fact]
        public void
            Should_configure_aspnetcore_listener_information_as_singleton_When_activating_replica_template()
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
                instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(IServiceHostAspNetCoreListenerInformation) == v.ServiceType)),
                Times.Once());
        }

        [Fact]
        public void
            Should_configure_aspnetcore_listener_information_with_endpoint_name_and_url_suffix_When_activating_replica_template()
        {
            // Arrange
            const string EndpointName = "EndpointName";

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

                    config.UseEndpointName(EndpointName);

                    // Generate random guid-like URI
                    config.UseIntegrationOptions(ServiceFabricIntegrationOptions.UseUniqueServiceUrl);
                });

            var listener = replicaTemplate.Activate(service);

            var invoker = this.CreateInvoker(listener);
            invoker.Invoke();

            // Assert
            serviceCollection.Verify(
                instance => instance.Add(
                    It.Is<ServiceDescriptor>(
                        v =>
                            typeof(IServiceHostAspNetCoreListenerInformation) == v.ServiceType
                         && ((IServiceHostAspNetCoreListenerInformation) v.ImplementationInstance).EndpointName == EndpointName
                         && ((IServiceHostAspNetCoreListenerInformation) v.ImplementationInstance).UrlSuffix != string.Empty
                    )),
                Times.Once());
        }

        [Fact]
        public void
            Should_configure_service_context_as_singleton_service_When_activating_replica_template()
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
                instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(ServiceContext) == v.ServiceType)),
                Times.Once());
        }

        [Fact]
        public void
            Should_configure_service_partition_as_singleton_When_activating_replica_template()
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
                instance => instance.Add(It.Is<ServiceDescriptor>(v => typeof(IServicePartition) == v.ServiceType)),
                Times.Once());
        }

        [Fact]
        public void
            Should_throw_FactoryProducesNullInstanceException_When_web_host_builder_func_returns_null()
        {
            // Arrange
            var service = this.CreateService();

            // Act
            var replicaTemplate = this.CreateInstance();
            replicaTemplate.ConfigureObject(
                config =>
                {
                    config.UseAspNetCoreCommunicationListener(AspNetCoreCommunicationListenerStub.Func);
                    config.UseWebHostBuilderExtensionsImpl(WebHostBuilderExtensionsImplStub.Func);
                    config.UseWebHostBuilder(() => null);
                });

            var listener = replicaTemplate.Activate(service);

            // Assert
            Assert.Throws<FactoryProducesNullInstanceException<IWebHostBuilder>>(
                () => this.CreateInvoker(listener).Invoke());
        }

        [Fact]
        public void
            Should_throw_InvalidOperationException_When_aspnetcore_communication_listener_func_isnot_configured()
        {
            // Arrange
            var service = this.CreateService();

            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    this.CreateInstance().Activate(service);
                });
        }

        [Fact]
        public void
            Should_use_custom_aspnetcore_communication_listener_func_When_aspnetcore_communication_listener_func_is_configured()
        {
            // Arrange
            var service = this.CreateService();

            var factory =
                new Mock<Func<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>, AspNetCoreCommunicationListener>>();
            factory
               .Setup(
                    instance => instance(
                        It.IsAny<ServiceContext>(),
                        It.IsAny<string>(),
                        It.IsAny<Func<string, AspNetCoreCommunicationListener, IWebHost>>()))
               .Returns<ServiceContext, string, Func<string, AspNetCoreCommunicationListener, IWebHost>>(
                    AspNetCoreCommunicationListenerStub.Func);

            // Act
            var replicaTemplate = this.CreateInstance();
            replicaTemplate.ConfigureObject(
                config =>
                {
                    config.UseAspNetCoreCommunicationListener(factory.Object);
                    config.UseWebHostBuilderExtensionsImpl(WebHostBuilderExtensionsImplStub.Func);
                    config.UseWebHostBuilder(WebHostBuilderStub.Func);
                });

            var listener = replicaTemplate.Activate(service);

            var invoker = this.CreateInvoker(listener);
            invoker.Invoke();

            // Assert
            factory.Verify(
                instance => instance(
                    It.IsAny<ServiceContext>(),
                    It.IsAny<string>(),
                    It.IsAny<Func<string, AspNetCoreCommunicationListener, IWebHost>>()),
                Times.Once());
        }

        [Fact]
        public void
            Should_use_custom_web_host_builder_func_When_web_host_builder_func_is_configured()
        {
            // Arrange
            var service = this.CreateService();

            var factory = new Mock<Func<IWebHostBuilder>>();
            factory
               .Setup(instance => instance())
               .Returns(new WebHostBuilderStub());

            // Act
            var replicaTemplate = this.CreateInstance();
            replicaTemplate.ConfigureObject(
                config =>
                {
                    config.UseAspNetCoreCommunicationListener(AspNetCoreCommunicationListenerStub.Func);
                    config.UseWebHostBuilderExtensionsImpl(WebHostBuilderExtensionsImplStub.Func);
                    config.UseWebHostBuilder(factory.Object);
                });

            var listener = replicaTemplate.Activate(service);

            var invoker = this.CreateInvoker(listener);
            invoker.Invoke();

            // Assert
            factory.Verify(instance => instance(), Times.Once());
        }

        [Fact]
        public void
            Should_use_service_fabric_integration_options_When_configuring_web_host()
        {
            // Arrange
            var service = this.CreateService();

            var impl = new Mock<IWebHostBuilderExtensionsImpl>();

            // Act
            var replicaTemplate = this.CreateInstance();
            replicaTemplate.ConfigureObject(
                config =>
                {
                    config.UseAspNetCoreCommunicationListener(AspNetCoreCommunicationListenerStub.Func);
                    config.UseWebHostBuilderExtensionsImpl(() => impl.Object);
                    config.UseWebHostBuilder(WebHostBuilderStub.Func);
                });

            var listener = replicaTemplate.Activate(service);

            var invoker = this.CreateInvoker(listener);
            invoker.Invoke();

            // Assert
            impl.Verify(
                instance => instance.UseServiceFabricIntegration(
                    It.IsAny<IWebHostBuilder>(),
                    It.IsAny<AspNetCoreCommunicationListener>(),
                    It.IsAny<ServiceFabricIntegrationOptions>()),
                Times.Once());
        }

        [Fact]
        public void
            Should_use_urls_When_configuring_web_host()
        {
            // Arrange
            var service = this.CreateService();

            var impl = new Mock<IWebHostBuilderExtensionsImpl>();

            // Act
            var replicaTemplate = this.CreateInstance();
            replicaTemplate.ConfigureObject(
                config =>
                {
                    config.UseAspNetCoreCommunicationListener(AspNetCoreCommunicationListenerStub.Func);
                    config.UseWebHostBuilderExtensionsImpl(() => impl.Object);
                    config.UseWebHostBuilder(WebHostBuilderStub.Func);
                });

            var listener = replicaTemplate.Activate(service);

            var invoker = this.CreateInvoker(listener);
            invoker.Invoke();

            // Assert
            impl.Verify(
                instance => instance.UseUrls(
                    It.IsAny<IWebHostBuilder>(),
                    It.IsAny<string>()),
                Times.Once());
        }
    }
}