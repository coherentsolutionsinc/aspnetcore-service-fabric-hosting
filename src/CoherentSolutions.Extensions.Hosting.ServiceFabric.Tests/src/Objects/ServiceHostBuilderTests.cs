using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Moq;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class StatefulServiceHostBuilderTests
        : ServiceHostBuilderTests<IStatefulServiceHost,
            IStatefulServiceHostBuilderParameters,
            IStatefulServiceHostBuilderConfigurator,
            IStatefulServiceHostEventSourceReplicableTemplate,
            IStatefulServiceHostEventSourceReplicaTemplate,
            IStatefulServiceHostEventSourceReplicaTemplateConfigurator,
            IStatefulServiceHostEventSourceReplicator,
            IStatefulServiceHostDelegateReplicableTemplate,
            IStatefulServiceHostDelegateReplicaTemplate,
            IStatefulServiceHostDelegateReplicaTemplateConfigurator,
            IStatefulServiceHostDelegateReplicator,
            IStatefulServiceHostListenerReplicableTemplate,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplate,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
            IStatefulServiceHostRemotingListenerReplicaTemplate,
            IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator,
            IStatefulServiceHostGenericListenerReplicaTemplate,
            IStatefulServiceHostGenericListenerReplicaTemplateConfigurator,
            IStatefulServiceHostListenerReplicator
        >
    {
        protected override ServiceHostBuilder<
                IStatefulServiceHost,
                IStatefulServiceHostBuilderParameters,
                IStatefulServiceHostBuilderConfigurator,
                IStatefulServiceHostEventSourceReplicableTemplate,
                IStatefulServiceHostEventSourceReplicaTemplate,
                IStatefulServiceHostEventSourceReplicator,
                IStatefulServiceHostDelegateReplicableTemplate,
                IStatefulServiceHostDelegateReplicaTemplate,
                IStatefulServiceHostDelegateReplicator,
                IStatefulServiceHostListenerReplicableTemplate,
                IStatefulServiceHostAspNetCoreListenerReplicaTemplate,
                IStatefulServiceHostRemotingListenerReplicaTemplate,
                IStatefulServiceHostGenericListenerReplicaTemplate,
                IStatefulServiceHostListenerReplicator
            >
            CreateServiceInstance()
        {
            return new StatefulServiceHostBuilder();
        }
    }

    public class StatelessServiceHostBuilderTests
        : ServiceHostBuilderTests<IStatelessServiceHost,
            IStatelessServiceHostBuilderParameters,
            IStatelessServiceHostBuilderConfigurator,
            IStatelessServiceHostEventSourceReplicableTemplate,
            IStatelessServiceHostEventSourceReplicaTemplate,
            IStatelessServiceHostEventSourceReplicaTemplateConfigurator,
            IStatelessServiceHostEventSourceReplicator,
            IStatelessServiceHostDelegateReplicableTemplate,
            IStatelessServiceHostDelegateReplicaTemplate,
            IStatelessServiceHostDelegateReplicaTemplateConfigurator,
            IStatelessServiceHostDelegateReplicator,
            IStatelessServiceHostListenerReplicableTemplate,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplate,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
            IStatelessServiceHostRemotingListenerReplicaTemplate,
            IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator,
            IStatelessServiceHostGenericListenerReplicaTemplate,
            IStatelessServiceHostGenericListenerReplicaTemplateConfigurator,
            IStatelessServiceHostListenerReplicator
        >
    {
        protected override ServiceHostBuilder<
                IStatelessServiceHost,
                IStatelessServiceHostBuilderParameters,
                IStatelessServiceHostBuilderConfigurator,
                IStatelessServiceHostEventSourceReplicableTemplate,
                IStatelessServiceHostEventSourceReplicaTemplate,
                IStatelessServiceHostEventSourceReplicator,
                IStatelessServiceHostDelegateReplicableTemplate,
                IStatelessServiceHostDelegateReplicaTemplate,
                IStatelessServiceHostDelegateReplicator,
                IStatelessServiceHostListenerReplicableTemplate,
                IStatelessServiceHostAspNetCoreListenerReplicaTemplate,
                IStatelessServiceHostRemotingListenerReplicaTemplate,
                IStatelessServiceHostGenericListenerReplicaTemplate,
                IStatelessServiceHostListenerReplicator
            >
            CreateServiceInstance()
        {
            return new StatelessServiceHostBuilder();
        }
    }

    public abstract class ServiceHostBuilderTests<
        TServiceHost,
        TParameters,
        TConfigurator,
        TEventSourceReplicableTemplate,
        TEventSourceReplicaTemplate,
        TEventSourceReplicaTemplateConfigurator,
        TEventSourceReplicator,
        TDelegateReplicableTemplate,
        TDelegateReplicaTemplate,
        TDelegateReplicaTemplateConfigurator,
        TDelegateReplicator,
        TListenerReplicableTemplate,
        TListenerAspNetCoreReplicaTemplate,
        TListenerAspNetCoreReplicaTemplateConfigurator,
        TListenerRemotingReplicaTemplate,
        TListenerRemotingReplicaTemplateConfigurator,
        TListenerGenericReplicaTemplate,
        TListenerGenericReplicaTemplateConfigurator,
        TListenerReplicator>
        where TParameters :
        class,
        IServiceHostBuilderParameters,
        IServiceHostBuilderEventSourceParameters<TEventSourceReplicaTemplate>,
        IServiceHostBuilderEventSourceReplicationParameters<TEventSourceReplicableTemplate, TEventSourceReplicator>,
        IServiceHostBuilderDelegateParameters<TDelegateReplicaTemplate>,
        IServiceHostBuilderDelegateReplicationParameters<TDelegateReplicableTemplate, TDelegateReplicator>,
        IServiceHostBuilderAspNetCoreListenerParameters<TListenerAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerParameters<TListenerRemotingReplicaTemplate>,
        IServiceHostBuilderGenericListenerParameters<TListenerGenericReplicaTemplate>,
        IServiceHostBuilderListenerReplicationParameters<TListenerReplicableTemplate, TListenerReplicator>
        where TConfigurator :
        class,
        IServiceHostBuilderConfigurator,
        IServiceHostBuilderEventSourceConfigurator<TEventSourceReplicaTemplate>,
        IServiceHostBuilderEventSourceReplicationConfigurator<TEventSourceReplicableTemplate, TEventSourceReplicator>,
        IServiceHostBuilderDelegateConfigurator<TDelegateReplicaTemplate>,
        IServiceHostBuilderDelegateReplicationConfigurator<TDelegateReplicableTemplate, TDelegateReplicator>,
        IServiceHostBuilderAspNetCoreListenerConfigurator<TListenerAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerConfigurator<TListenerRemotingReplicaTemplate>,
        IServiceHostBuilderGenericListenerConfigurator<TListenerGenericReplicaTemplate>,
        IServiceHostBuilderListenerReplicationConfigurator<TListenerReplicableTemplate, TListenerReplicator>
        where TEventSourceReplicaTemplate :
        class,
        TEventSourceReplicableTemplate,
        IServiceHostEventSourceReplicaTemplate<TEventSourceReplicaTemplateConfigurator>
        where TEventSourceReplicaTemplateConfigurator :
        class,
        IServiceHostEventSourceReplicaTemplateConfigurator
        where TEventSourceReplicator :
        class
        where TDelegateReplicaTemplate :
        class,
        TDelegateReplicableTemplate,
        IServiceHostDelegateReplicaTemplate<TDelegateReplicaTemplateConfigurator>
        where TDelegateReplicaTemplateConfigurator :
        class,
        IServiceHostDelegateReplicaTemplateConfigurator
        where TDelegateReplicator :
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
        where TListenerGenericReplicaTemplate :
        class,
        TListenerReplicableTemplate,
        IServiceHostGenericListenerReplicaTemplate<TListenerGenericReplicaTemplateConfigurator>
        where TListenerGenericReplicaTemplateConfigurator :
        class,
        IServiceHostGenericListenerReplicaTemplateConfigurator
        where TListenerReplicator :
        class
    {
        protected abstract ServiceHostBuilder<TServiceHost,
            TParameters,
            TConfigurator,
            TEventSourceReplicableTemplate,
            TEventSourceReplicaTemplate,
            TEventSourceReplicator,
            TDelegateReplicableTemplate,
            TDelegateReplicaTemplate,
            TDelegateReplicator,
            TListenerReplicableTemplate,
            TListenerAspNetCoreReplicaTemplate,
            TListenerRemotingReplicaTemplate,
            TListenerGenericReplicaTemplate,
            TListenerReplicator
        > CreateServiceInstance();

        [Fact]
        public void
            Should_use_aspnetcore_listener_configuration_action_When_configuring_aspnetcore_listener()
        {
            // Arrange
            var action = new Mock<Action<TListenerAspNetCoreReplicaTemplate>>();
            action
               .Setup(instance => instance(It.IsAny<TListenerAspNetCoreReplicaTemplate>()));

            // Act
            var builder = this.CreateServiceInstance();
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
            Should_use_aspnetcore_listener_replica_template_func_When_configuring_aspnetcore_listener()
        {
            // Arrange
            var factory = new Mock<Func<TListenerAspNetCoreReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns(new Mock<TListenerAspNetCoreReplicaTemplate>().Object);

            // Act
            var builder = this.CreateServiceInstance();
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
            Should_use_delegate_configuration_action_When_configuring_delegate()
        {
            // Arrange
            var action = new Mock<Action<TDelegateReplicableTemplate>>();
            action
               .Setup(instance => instance(It.IsAny<TDelegateReplicableTemplate>()));

            // Act
            var builder = this.CreateServiceInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.DefineDelegate(action.Object);
                });
            builder.Build();

            // Assert
            action.Verify(instance => instance(It.IsAny<TDelegateReplicableTemplate>()), Times.Once());
        }

        [Fact]
        public void
            Should_use_delegate_replicator_func_When_configuring_service()
        {
            // Arrange
            var factory = new Mock<Func<TDelegateReplicableTemplate, TDelegateReplicator>>();
            factory
               .Setup(instance => instance(It.IsAny<TDelegateReplicableTemplate>()))
               .Returns(new Mock<TDelegateReplicator>().Object);

            // Act
            var builder = this.CreateServiceInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.UseDelegateReplicator(factory.Object);
                    config.DefineDelegate(
                        c =>
                        {
                        });
                });
            builder.Build();

            // Assert
            factory.Verify(instance => instance(It.IsAny<TDelegateReplicableTemplate>()), Times.Once());
        }

        [Fact]
        public void
            Should_use_event_source_func_When_configuring_service()
        {
            // Arrange
            var factory = new Mock<Func<TEventSourceReplicableTemplate, TEventSourceReplicator>>();
            factory
               .Setup(instance => instance(It.IsAny<TEventSourceReplicableTemplate>()))
               .Returns(new Mock<TEventSourceReplicator>().Object);

            // Act
            var builder = this.CreateServiceInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.UseEventSourceReplicator(factory.Object);
                    config.DefineAspNetCoreListener(
                        c =>
                        {
                        });
                    config.DefineRemotingListener(
                        c =>
                        {
                        });
                });
            builder.Build();

            // Assert
            factory.Verify(instance => instance(It.IsAny<TEventSourceReplicableTemplate>()), Times.Once());
        }

        [Fact]
        public void
            Should_use_generic_listener_configuration_action_When_configuring_generic_listener()
        {
            // Arrange
            var action = new Mock<Action<TListenerGenericReplicaTemplate>>();
            action
               .Setup(instance => instance(It.IsAny<TListenerGenericReplicaTemplate>()));

            // Act
            var builder = this.CreateServiceInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.DefineGenericListener(action.Object);
                });
            builder.Build();

            // Assert
            action.Verify(instance => instance(It.IsAny<TListenerGenericReplicaTemplate>()), Times.Once());
        }

        [Fact]
        public void
            Should_use_generic_listener_replica_template_func_When_configuring_generic_listener()
        {
            // Arrange
            var factory = new Mock<Func<TListenerGenericReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns(new Mock<TListenerGenericReplicaTemplate>().Object);

            // Act
            var builder = this.CreateServiceInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.UseGenericListenerReplicaTemplate(factory.Object);
                    config.DefineGenericListener(
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
            Should_use_listener_replicator_func_When_configuring_service()
        {
            // Arrange
            var factory = new Mock<Func<TListenerReplicableTemplate, TListenerReplicator>>();
            factory
               .Setup(instance => instance(It.IsAny<TListenerReplicableTemplate>()))
               .Returns(new Mock<TListenerReplicator>().Object);

            // Act
            var builder = this.CreateServiceInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.UseListenerReplicator(factory.Object);
                    config.DefineAspNetCoreListener(
                        c =>
                        {
                        });
                    config.DefineRemotingListener(
                        c =>
                        {
                        });
                });
            builder.Build();

            // Assert
            factory.Verify(instance => instance(It.IsAny<TListenerReplicableTemplate>()), Times.Exactly(2));
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
            var builder = this.CreateServiceInstance();
            builder.ConfigureObject(
                config =>
                {
                    config.DefineRemotingListener(action.Object);
                });
            builder.Build();

            // Assert
            action.Verify(instance => instance(It.IsAny<TListenerRemotingReplicaTemplate>()), Times.Once());
        }

        [Fact]
        public void
            Should_use_remoting_listener_replica_template_func_When_configuring_remoting_listener()
        {
            // Arrange
            var factory = new Mock<Func<TListenerRemotingReplicaTemplate>>();
            factory
               .Setup(instance => instance())
               .Returns(new Mock<TListenerRemotingReplicaTemplate>().Object);

            // Act
            var builder = this.CreateServiceInstance();
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
    }
}