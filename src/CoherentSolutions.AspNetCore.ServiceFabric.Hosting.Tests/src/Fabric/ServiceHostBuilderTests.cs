using System;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common.Exceptions;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Fabric
{
    public abstract class ServiceHostBuilderTests<
        TServiceHost,
        TParameters,
        TConfigurator,
        TReplicableTemplate,
        TAspNetCoreReplicaTemplate,
        TAspNetCoreReplicaTemplateConfigurator,
        TReplicator>
        where TParameters :
        class,
        IServiceHostBuilderParameters,
        IServiceHostBuilderAspNetCoreListenerParameters<TAspNetCoreReplicaTemplate>,
        IServiceHostBuilderListenerReplicationParameters<TReplicableTemplate, TReplicator>
        where TConfigurator :
        class,
        IServiceHostBuilderConfigurator,
        IServiceHostBuilderAspNetCoreListenerConfigurator<TAspNetCoreReplicaTemplate>,
        IServiceHostBuilderListenerReplicationConfigurator<TReplicableTemplate, TReplicator>
        where TAspNetCoreReplicaTemplate :
        class,
        TReplicableTemplate,
        IServiceHostAspNetCoreListenerReplicaTemplate<TAspNetCoreReplicaTemplateConfigurator>
        where TAspNetCoreReplicaTemplateConfigurator :
        class,
        IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        where TReplicator :
        class
    {
        protected abstract ServiceHostBuilder<TServiceHost,
            TParameters,
            TConfigurator,
            TReplicableTemplate,
            TAspNetCoreReplicaTemplate,
            TReplicator
        > CreateInstance();

        [Fact]
        public void
            Should_throw_FactoryProducesNullInstanceException_When_aspnetcore_replica_template_func_returns_null()
        {
            // Arrange
            var factory = new Mock<Func<TAspNetCoreReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns<TAspNetCoreReplicaTemplate>(null);

            // Act
            var builder = this.CreateInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.UseAspNetCoreListenerReplicaTemplate(factory.Object);
                    config.DefineAspNetCoreListener(
                        c =>
                        {
                        });
                });

            // Assert
            Assert.Throws<FactoryProducesNullInstanceException<TAspNetCoreReplicaTemplate>>(() => builder.Build());
        }

        [Fact]
        public void
            Should_throw_FactoryProducesNullInstanceException_When_replicator_func_returns_null()
        {
            // Arrange
            var factory = new Mock<Func<TReplicableTemplate, TReplicator>>();
            factory
               .Setup(instance => instance(It.IsAny<TReplicableTemplate>()))
               .Returns<TReplicator>(null);

            // Act
            var builder = this.CreateInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.UseListenerReplicator(factory.Object);
                    config.DefineAspNetCoreListener(
                        c =>
                        {
                        });
                });

            // Assert
            Assert.Throws<FactoryProducesNullInstanceException<TReplicator>>(() => builder.Build());
        }

        [Fact]
        public void
            Should_use_aspnetcore_listener_configuration_action_When_configuring_aspnetcore_listener()
        {
            // Arrange
            var action = new Mock<Action<TAspNetCoreReplicaTemplate>>();
            action
               .Setup(instance => instance(It.IsAny<TAspNetCoreReplicaTemplate>()));

            // Act
            var builder = this.CreateInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.DefineAspNetCoreListener(action.Object);
                });
            builder.Build();

            // Assert
            action.Verify(instance => instance(It.IsAny<TAspNetCoreReplicaTemplate>()), Times.Once());
        }

        [Fact]
        public void
            Should_use_custom_aspnetcore_replica_template_func_When_aspnetcore_replica_template_func_is_configured()
        {
            // Arrange
            var factory = new Mock<Func<TAspNetCoreReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns(new Mock<TAspNetCoreReplicaTemplate>().Object);

            // Act
            var builder = this.CreateInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.UseAspNetCoreListenerReplicaTemplate(factory.Object);
                    config.DefineAspNetCoreListener(
                        c =>
                        {
                        });
                });
            builder.Build();

            // Assert
            factory.Verify(instance => instance(), Times.Once());
        }

        [Fact]
        public void
            Should_use_custom_replicator_func_When_replicator_func_is_configured()
        {
            // Arrange
            var factory = new Mock<Func<TReplicableTemplate, TReplicator>>();
            factory
               .Setup(instance => instance(It.IsAny<TReplicableTemplate>()))
               .Returns(new Mock<TReplicator>(MockBehavior.Loose).Object);

            // Act
            var builder = this.CreateInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.UseListenerReplicator(factory.Object);
                    config.DefineAspNetCoreListener(
                        c =>
                        {
                        });
                });
            builder.Build();

            // Assert
            factory.Verify(instance => instance(It.IsAny<TReplicableTemplate>()), Times.Once());
        }

        [Fact]
        public void
            Should_web_host_builder_func_from_service_When_configuring_aspnetcore_listener_without_web_host_builder_func()
        {
            // Arrange
            var webfactory = new Mock<Func<IWebHostBuilder>>();

            var configurator = new Mock<TAspNetCoreReplicaTemplateConfigurator>();

            var builder = new Mock<TAspNetCoreReplicaTemplate>();
            builder
               .Setup(instance => instance.ConfigureObject(It.Is<Action<TAspNetCoreReplicaTemplateConfigurator>>(v => true)))
               .Callback<Action<TAspNetCoreReplicaTemplateConfigurator>>(action => action(configurator.Object));

            var factory = new Mock<Func<TAspNetCoreReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns(builder.Object);

            // Act
            var configurable = this.CreateInstance();
            configurable.ConfigureObject(
                config =>
                {
                    config.UseWebHostBuilder(webfactory.Object);
                    config.UseAspNetCoreListenerReplicaTemplate(factory.Object);
                    config.DefineAspNetCoreListener(
                        c =>
                        {
                        });
                });
            configurable.Build();

            // Assert
            configurator.Verify(instance => instance.UseWebHostBuilder(webfactory.Object), Times.Once());
        }
    }
}