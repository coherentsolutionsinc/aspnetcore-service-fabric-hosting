using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public abstract class ServiceHostListenerReplicaTemplateTests<TService, TParameters, TConfigurator, TListener>
        : ConfigurableObject<TConfigurator>, IServiceHostListenerReplicaTemplate<TConfigurator>
        where TService : IService
        where TParameters : IServiceHostListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostListenerReplicaTemplateConfigurator
    {
        protected abstract ServiceHostListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener> CreateReplicaTemplateInstance();

        protected abstract TService CreateServiceInstance();

        protected abstract string GetListenerName(
            TListener listener);

        [Fact]
        public void Should_set_listeners_name_to_endpoint_name_When_endpoint_name_is_configured()
        {
            // Arrange
            var arrangeEndpointName = "endpoint-name";

            var arrangeReplicableTemplate = this.CreateReplicaTemplateInstance();

            // Act
            arrangeReplicableTemplate.ConfigureObject(
                c =>
                {
                    c.UseEndpoint(arrangeEndpointName);
                });

            var listener = arrangeReplicableTemplate.Activate(this.CreateServiceInstance());

            // Assert
            Assert.Same(arrangeEndpointName, this.GetListenerName(listener));
        }
    }

    public abstract class StatefulServiceHostListenerReplicaTemplateTests<TParameters, TConfigurator>
        : ServiceHostListenerReplicaTemplateTests<IStatefulService, TParameters, TConfigurator, ServiceReplicaListener>
        where TParameters : IServiceHostListenerReplicaTemplateParameters, IStatefulServiceHostListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostListenerReplicaTemplateConfigurator, IStatefulServiceHostListenerReplicaTemplateConfigurator
    {
        protected sealed override IStatefulService CreateServiceInstance()
        {
            return new MockStatefulService();
        }

        protected sealed override string GetListenerName(
            ServiceReplicaListener listener)
        {
            return listener.Name;
        }

        [Fact]
        public void Should_set_listeners_listen_on_secondary_When_listen_on_secondary_is_configured()
        {
            // Arrange
            var arrangeReplicableTemplate = this.CreateReplicaTemplateInstance();

            // Act
            arrangeReplicableTemplate.ConfigureObject(
                c =>
                {
                    c.UseListenerOnSecondary();
                });

            var listener = arrangeReplicableTemplate.Activate(this.CreateServiceInstance());

            // Assert
            Assert.True(listener.ListenOnSecondary);
        }
    }

    public class StatefulServiceHostAspNetCoreListenerReplicaTemplateTests
        : StatefulServiceHostListenerReplicaTemplateTests<
            IStatefulServiceHostAspNetCoreListenerReplicaTemplateParameters,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
    {
        protected override ServiceHostListenerReplicaTemplate<
                IStatefulService,
                IStatefulServiceHostAspNetCoreListenerReplicaTemplateParameters,
                IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
                ServiceReplicaListener>
            CreateReplicaTemplateInstance()
        {
            return new StatefulServiceHostAspNetCoreListenerReplicaTemplate();
        }
    }

    public class StatefulServiceHostRemotingListenerReplicaTemplateTests
        : StatefulServiceHostListenerReplicaTemplateTests<
            IStatefulServiceHostRemotingListenerReplicaTemplateParameters,
            IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator>
    {
        protected override ServiceHostListenerReplicaTemplate<
                IStatefulService,
                IStatefulServiceHostRemotingListenerReplicaTemplateParameters,
                IStatefulServiceHostRemotingListenerReplicaTemplateConfigurator,
                ServiceReplicaListener>
            CreateReplicaTemplateInstance()
        {
            var replicaTemplate = new StatefulServiceHostRemotingListenerReplicaTemplate();
            replicaTemplate.UseImplementation<MockServiceRemotingListenerImplementation>();

            return replicaTemplate;
        }
    }

    public class StatefulServiceHostGenericListenerReplicaTemplateTests
        : StatefulServiceHostListenerReplicaTemplateTests<
            IStatefulServiceHostGenericListenerReplicaTemplateParameters,
            IStatefulServiceHostGenericListenerReplicaTemplateConfigurator>
    {
        protected override ServiceHostListenerReplicaTemplate<
                IStatefulService,
                IStatefulServiceHostGenericListenerReplicaTemplateParameters,
                IStatefulServiceHostGenericListenerReplicaTemplateConfigurator,
                ServiceReplicaListener>
            CreateReplicaTemplateInstance()
        {
            var replicaTemplate = new StatefulServiceHostGenericListenerReplicaTemplate();
            replicaTemplate.UseCommunicationListener(
                (
                    context,
                    name,
                    provider) => new MockGenericCommunicationListener());

            return replicaTemplate;
        }
    }

    public abstract class StatelessServiceHostListenerReplicaTemplateTests<TParameters, TConfigurator>
        : ServiceHostListenerReplicaTemplateTests<IStatelessService, TParameters, TConfigurator, ServiceInstanceListener>
        where TParameters : IServiceHostListenerReplicaTemplateParameters, IStatelessServiceHostListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostListenerReplicaTemplateConfigurator, IStatelessServiceHostListenerReplicaTemplateConfigurator
    {
        protected sealed override IStatelessService CreateServiceInstance()
        {
            return new MockStatelessService();
        }

        protected sealed override string GetListenerName(
            ServiceInstanceListener listener)
        {
            return listener.Name;
        }
    }

    public class StatelessServiceHostAspNetCoreListenerReplicaTemplateTests
        : StatelessServiceHostListenerReplicaTemplateTests<
            IStatelessServiceHostAspNetCoreListenerReplicaTemplateParameters,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
    {
        protected override ServiceHostListenerReplicaTemplate<
                IStatelessService,
                IStatelessServiceHostAspNetCoreListenerReplicaTemplateParameters,
                IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
                ServiceInstanceListener>
            CreateReplicaTemplateInstance()
        {
            return new StatelessServiceHostAspNetCoreListenerReplicaTemplate();
        }
    }

    public class StatelessServiceHostRemotingListenerReplicaTemplateTests
        : StatelessServiceHostListenerReplicaTemplateTests<
            IStatelessServiceHostRemotingListenerReplicaTemplateParameters,
            IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator>
    {
        protected override ServiceHostListenerReplicaTemplate<
                IStatelessService,
                IStatelessServiceHostRemotingListenerReplicaTemplateParameters,
                IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator,
                ServiceInstanceListener>
            CreateReplicaTemplateInstance()
        {
            var replicaTemplate = new StatelessServiceHostRemotingListenerReplicaTemplate();
            replicaTemplate.UseImplementation<MockServiceRemotingListenerImplementation>();

            return replicaTemplate;
        }
    }

    public class StatelessServiceHostGenericListenerReplicaTemplateTests
        : StatelessServiceHostListenerReplicaTemplateTests<
            IStatelessServiceHostGenericListenerReplicaTemplateParameters,
            IStatelessServiceHostGenericListenerReplicaTemplateConfigurator>
    {
        protected override ServiceHostListenerReplicaTemplate<
                IStatelessService,
                IStatelessServiceHostGenericListenerReplicaTemplateParameters,
                IStatelessServiceHostGenericListenerReplicaTemplateConfigurator,
                ServiceInstanceListener>
            CreateReplicaTemplateInstance()
        {
            var replicaTemplate = new StatelessServiceHostGenericListenerReplicaTemplate();
            replicaTemplate.UseCommunicationListener(
                (
                    context,
                    name,
                    provider) => new MockGenericCommunicationListener());

            return replicaTemplate;
        }
    }
}