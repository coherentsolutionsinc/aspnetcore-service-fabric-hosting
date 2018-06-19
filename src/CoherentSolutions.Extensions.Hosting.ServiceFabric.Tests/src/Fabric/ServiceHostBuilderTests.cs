using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Fabric
{
    public abstract class ServiceHostBuilderTests<
        TServiceHost,
        TParameters,
        TConfigurator,
        TAsyncDelegateReplicableTemplate,
        TAsyncDelegateReplicaTemplate,
        TAsyncDelegateReplicaTemplateConfigurator,
        TAsyncDelegateReplicator,
        TListenerReplicableTemplate,
        TListenerAspNetCoreReplicaTemplate,
        TListenerAspNetCoreReplicaTemplateConfigurator,
        TListenerRemotingReplicaTemplate,
        TListenerRemotingReplicaTemplateConfigurator,
        TListenerReplicator>
        where TParameters :
        class,
        IServiceHostBuilderParameters,
        IServiceHostBuilderDelegateParameters<TAsyncDelegateReplicaTemplate>,
        IServiceHostBuilderDelegateReplicationParameters<TAsyncDelegateReplicableTemplate, TAsyncDelegateReplicator>,
        IServiceHostBuilderAspNetCoreListenerParameters<TListenerAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerParameters<TListenerRemotingReplicaTemplate>,
        IServiceHostBuilderListenerReplicationParameters<TListenerReplicableTemplate, TListenerReplicator>
        where TConfigurator :
        class,
        IServiceHostBuilderConfigurator,
        IServiceHostBuilderDelegateConfigurator<TAsyncDelegateReplicaTemplate>,
        IServiceHostBuilderDelegateReplicationConfigurator<TAsyncDelegateReplicableTemplate, TAsyncDelegateReplicator>,
        IServiceHostBuilderAspNetCoreListenerConfigurator<TListenerAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerConfigurator<TListenerRemotingReplicaTemplate>,
        IServiceHostBuilderListenerReplicationConfigurator<TListenerReplicableTemplate, TListenerReplicator>
        where TAsyncDelegateReplicaTemplate :
        class,
        TAsyncDelegateReplicableTemplate,
        IServiceHostDelegateReplicaTemplate<TAsyncDelegateReplicaTemplateConfigurator>
        where TAsyncDelegateReplicaTemplateConfigurator :
        class,
        IServiceHostDelegateReplicaTemplateConfigurator
        where TAsyncDelegateReplicator :
        class
        where TListenerAspNetCoreReplicaTemplate :
        class,
        TListenerReplicableTemplate,
        IServiceHostAspNetCoreListenerReplicaTemplate<TListenerAspNetCoreReplicaTemplateConfigurator>
        where TListenerAspNetCoreReplicaTemplateConfigurator :
        class,
        IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
        where TListenerRemotingReplicaTemplate :
        class,
        TListenerReplicableTemplate,
        IServiceHostRemotingListenerReplicaTemplate<TListenerRemotingReplicaTemplateConfigurator>
        where TListenerRemotingReplicaTemplateConfigurator :
        class,
        IServiceHostRemotingListenerReplicaTemplateConfigurator
        where TListenerReplicator :
        class
    {
        protected abstract ServiceHostBuilder<TServiceHost,
            TParameters,
            TConfigurator,
            TAsyncDelegateReplicableTemplate,
            TAsyncDelegateReplicaTemplate,
            TAsyncDelegateReplicator,
            TListenerReplicableTemplate,
            TListenerAspNetCoreReplicaTemplate,
            TListenerRemotingReplicaTemplate,
            TListenerReplicator
        > CreateInstance();

        [Fact]
        public void
            Should_throw_FactoryProducesNullInstanceException_When_aspnetcore_replica_template_func_returns_null()
        {
            // Arrange
            var factory = new Mock<Func<TListenerAspNetCoreReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns<TListenerAspNetCoreReplicaTemplate>(null);

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
            Assert.Throws<FactoryProducesNullInstanceException<TListenerAspNetCoreReplicaTemplate>>(() => builder.Build());
        }

        [Fact]
        public void
            Should_throw_FactoryProducesNullInstanceException_When_remoting_replica_template_func_returns_null()
        {
            // Arrange
            var factory = new Mock<Func<TListenerRemotingReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns<TListenerRemotingReplicaTemplate>(null);

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
            Assert.Throws<FactoryProducesNullInstanceException<TListenerRemotingReplicaTemplate>>(() => builder.Build());
        }

        [Fact]
        public void
            Should_throw_FactoryProducesNullInstanceException_When_replicator_func_returns_null()
        {
            // Arrange
            var factory = new Mock<Func<TListenerReplicableTemplate, TListenerReplicator>>();
            factory
               .Setup(instance => instance(It.IsAny<TListenerReplicableTemplate>()))
               .Returns<TListenerReplicator>(null);

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
            Assert.Throws<FactoryProducesNullInstanceException<TListenerReplicator>>(() => builder.Build());
        }

        [Fact]
        public void
            Should_use_aspnetcore_listener_configuration_action_When_configuring_aspnetcore_listener()
        {
            // Arrange
            var action = new Mock<Action<TListenerAspNetCoreReplicaTemplate>>();
            action
               .Setup(instance => instance(It.IsAny<TListenerAspNetCoreReplicaTemplate>()));

            // Act
            var builder = this.CreateInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.DefineAspNetCoreListener(action.Object);
                });
            builder.Build();

            // Assert
            action.Verify(instance => instance(It.IsAny<TListenerAspNetCoreReplicaTemplate>()), Times.Once());
        }

        [Fact]
        public void
            Should_use_custom_aspnetcore_replica_template_func_When_aspnetcore_replica_template_func_is_configured()
        {
            // Arrange
            var factory = new Mock<Func<TListenerAspNetCoreReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns(new Mock<TListenerAspNetCoreReplicaTemplate>().Object);

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
            var factory = new Mock<Func<TListenerRemotingReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns(new Mock<TListenerRemotingReplicaTemplate>().Object);

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
            var factory = new Mock<Func<TListenerReplicableTemplate, TListenerReplicator>>();
            factory
               .Setup(instance => instance(It.IsAny<TListenerReplicableTemplate>()))
               .Returns(new Mock<TListenerReplicator>().Object);

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
            factory.Verify(instance => instance(It.IsAny<TListenerReplicableTemplate>()), Times.Once());
        }

        [Fact]
        public void
            Should_use_remoting_listener_configuration_action_When_configuring_remoting_listener()
        {
            // Arrange
            var action = new Mock<Action<TListenerRemotingReplicaTemplate>>();
            action
               .Setup(instance => instance(It.IsAny<TListenerRemotingReplicaTemplate>()));

            // Act
            var builder = this.CreateInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.DefineRemotingListener(action.Object);
                });
            builder.Build();

            // Assert
            action.Verify(instance => instance(It.IsAny<TListenerRemotingReplicaTemplate>()), Times.Once());
        }
    }
}