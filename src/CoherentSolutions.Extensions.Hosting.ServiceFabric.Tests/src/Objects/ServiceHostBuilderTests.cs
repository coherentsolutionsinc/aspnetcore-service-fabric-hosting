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
            IStatefulServiceHostDelegateReplicableTemplate,
            IStatefulServiceHostDelegateReplicaTemplate,
            IStatefulServiceHostDelegateReplicaTemplateConfigurator,
            IStatefulServiceHostDelegateReplicator,
            IStatefulServiceHostListenerReplicableTemplate,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplate,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
            IStatefulServiceHostRemotingListenerReplicaTemplate,
            IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator,
            IStatefulServiceHostListenerReplicator
        >
    {
        protected override ServiceHostBuilder<
                IStatefulServiceHost,
                IStatefulServiceHostBuilderParameters,
                IStatefulServiceHostBuilderConfigurator,
                IStatefulServiceHostDelegateReplicableTemplate,
                IStatefulServiceHostDelegateReplicaTemplate,
                IStatefulServiceHostDelegateReplicator,
                IStatefulServiceHostListenerReplicableTemplate,
                IStatefulServiceHostAspNetCoreListenerReplicaTemplate,
                IStatefulServiceHostRemotingListenerReplicaTemplate,
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
            IStatelessServiceHostDelegateReplicableTemplate,
            IStatelessServiceHostDelegateReplicaTemplate,
            IStatelessServiceHostDelegateReplicaTemplateConfigurator,
            IStatelessServiceHostDelegateReplicator,
            IStatelessServiceHostListenerReplicableTemplate,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplate,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
            IStatelessServiceHostRemotingListenerReplicaTemplate,
            IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator,
            IStatelessServiceHostListenerReplicator
        >
    {
        protected override ServiceHostBuilder<
                IStatelessServiceHost,
                IStatelessServiceHostBuilderParameters,
                IStatelessServiceHostBuilderConfigurator,
                IStatelessServiceHostDelegateReplicableTemplate,
                IStatelessServiceHostDelegateReplicaTemplate,
                IStatelessServiceHostDelegateReplicator,
                IStatelessServiceHostListenerReplicableTemplate,
                IStatelessServiceHostAspNetCoreListenerReplicaTemplate,
                IStatelessServiceHostRemotingListenerReplicaTemplate,
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
        TDelegateReplicableTemplate,
        TDelegateReplicaTemplate,
        TDelegateReplicaTemplateConfigurator,
        TDelegateReplicator,
        TListenerReplicableTemplate,
        TListenerAspNetCoreReplicaTemplate,
        TListenerAspNetCoreReplicaTemplateConfigurator,
        TListenerRemotingReplicaTemplate,
        TListenerRemotingReplicaTemplateConfigurator,
        TListenerReplicator>
        where TParameters :
        class,
        IServiceHostBuilderParameters,
        IServiceHostBuilderDelegateParameters<TDelegateReplicaTemplate>,
        IServiceHostBuilderDelegateReplicationParameters<TDelegateReplicableTemplate, TDelegateReplicator>,
        IServiceHostBuilderAspNetCoreListenerParameters<TListenerAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerParameters<TListenerRemotingReplicaTemplate>,
        IServiceHostBuilderListenerReplicationParameters<TListenerReplicableTemplate, TListenerReplicator>
        where TConfigurator :
        class,
        IServiceHostBuilderConfigurator,
        IServiceHostBuilderDelegateConfigurator<TDelegateReplicaTemplate>,
        IServiceHostBuilderDelegateReplicationConfigurator<TDelegateReplicableTemplate, TDelegateReplicator>,
        IServiceHostBuilderAspNetCoreListenerConfigurator<TListenerAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerConfigurator<TListenerRemotingReplicaTemplate>,
        IServiceHostBuilderListenerReplicationConfigurator<TListenerReplicableTemplate, TListenerReplicator>
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
        where TListenerReplicator :
        class
    {
        protected abstract ServiceHostBuilder<TServiceHost,
            TParameters,
            TConfigurator,
            TDelegateReplicableTemplate,
            TDelegateReplicaTemplate,
            TDelegateReplicator,
            TListenerReplicableTemplate,
            TListenerAspNetCoreReplicaTemplate,
            TListenerRemotingReplicaTemplate,
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
    }
}