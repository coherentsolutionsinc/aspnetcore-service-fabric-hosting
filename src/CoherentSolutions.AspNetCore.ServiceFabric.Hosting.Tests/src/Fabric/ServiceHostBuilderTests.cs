using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common.Exceptions;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

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
        TRemotingReplicaTemplate,
        TRemotingReplicaTemplateConfigurator,
        TReplicator>
        where TParameters :
        class,
        IServiceHostBuilderParameters,
        IServiceHostBuilderAspNetCoreListenerParameters<TAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerParameters<TRemotingReplicaTemplate>,
        IServiceHostBuilderListenerReplicationParameters<TReplicableTemplate, TReplicator>
        where TConfigurator :
        class,
        IServiceHostBuilderConfigurator,
        IServiceHostBuilderAspNetCoreListenerConfigurator<TAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerConfigurator<TRemotingReplicaTemplate>,
        IServiceHostBuilderListenerReplicationConfigurator<TReplicableTemplate, TReplicator>
        where TAspNetCoreReplicaTemplate :
        class,
        TReplicableTemplate,
        IServiceHostAspNetCoreListenerReplicaTemplate<TAspNetCoreReplicaTemplateConfigurator>
        where TAspNetCoreReplicaTemplateConfigurator :
        class,
        IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        where TRemotingReplicaTemplate :
        class,
        TReplicableTemplate,
        IServiceHostRemotingListenerReplicaTemplate<TRemotingReplicaTemplateConfigurator>
        where TRemotingReplicaTemplateConfigurator :
        class,
        IServiceHostRemotingListenerReplicaTemplateConfigurator
        where TReplicator :
        class
    {
        protected abstract ServiceHostBuilder<TServiceHost,
            TParameters,
            TConfigurator,
            TReplicableTemplate,
            TAspNetCoreReplicaTemplate,
            TRemotingReplicaTemplate,
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
            Should_throw_FactoryProducesNullInstanceException_When_remoting_replica_template_func_returns_null()
        {
            // Arrange
            var factory = new Mock<Func<TRemotingReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns<TRemotingReplicaTemplate>(null);

            // Act
            var builder = this.CreateInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.UseRemotingListenerReplicaTemplate(factory.Object);
                    config.DefineRemotingListener(
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
            Should_use_custom_remoting_replica_template_func_When_remoting_replica_template_func_is_configured()
        {
            // Arrange
            var factory = new Mock<Func<TRemotingReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns(new Mock<TRemotingReplicaTemplate>().Object);

            // Act
            var builder = this.CreateInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.UseRemotingListenerReplicaTemplate(factory.Object);
                    config.DefineRemotingListener(
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
               .Returns(new Mock<TReplicator>().Object);

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
            Should_use_remoting_listener_configuration_action_When_configuring_remoting_listener()
        {
            // Arrange
            var action = new Mock<Action<TRemotingReplicaTemplate>>();
            action
               .Setup(instance => instance(It.IsAny<TRemotingReplicaTemplate>()));

            // Act
            var builder = this.CreateInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.DefineRemotingListener(action.Object);
                });
            builder.Build();

            // Assert
            action.Verify(instance => instance(It.IsAny<TRemotingReplicaTemplate>()), Times.Once());
        }
    }
}