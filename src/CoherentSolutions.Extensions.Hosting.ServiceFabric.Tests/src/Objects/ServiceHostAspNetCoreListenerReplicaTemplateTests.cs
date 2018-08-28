using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

using Xunit;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Objects
{
    public class ServiceHostAspNetCoreListenerReplicaTemplateTests
        : ServiceHostAspNetCoreListenerReplicaTemplateTests<
            IStatefulService, 
            IStatefulServiceHostAspNetCoreListenerReplicaTemplateParameters,
            IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
            ServiceReplicaListener>
    {
        [Fact]
        public static void Should_set_listeners_name_to_endpoint_name_When_endpoint_name_is_configured()
        {
            // Arrange
            string arrangeEndpointName = "endpoint-name";

            var arrangeReplicableTemplate = new StatefulServiceHostAspNetCoreListenerReplicaTemplate();

            // Act
            arrangeReplicableTemplate.ConfigureObject(
                c =>
                {
                    c.UseEndpoint(arrangeEndpointName);
                });

            var listener = arrangeReplicableTemplate.Activate(new MockStatefulService());

            // Assert
            Assert.Same(arrangeEndpointName, listener.Name);
        }

        [Fact]
        public static void Should_set_listeners_listen_on_secondary_When_listen_on_secondary_is_configured()
        {
            // Arrange
            var arrangeReplicableTemplate = new StatefulServiceHostAspNetCoreListenerReplicaTemplate();
            
            // Act
            arrangeReplicableTemplate.ConfigureObject(
                c =>
                {
                    c.UseListenerOnSecondary();
                });

            var listener = arrangeReplicableTemplate.Activate(new MockStatefulService());

            // Assert
            Assert.True(listener.ListenOnSecondary);
        }
    }
    
    public class StatelessServiceHostAspNetCoreListenerReplicaTemplateTests
        : ServiceHostAspNetCoreListenerReplicaTemplateTests<
            IStatelessService, 
            IStatelessServiceHostAspNetCoreListenerReplicaTemplateParameters,
            IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator,
            ServiceInstanceListener>
    {
        [Fact]
        public static void Should_set_listeners_name_to_endpoint_name_When_endpoint_name_is_configured()
        {
            // Arrange
            string arrangeEndpointName = "endpoint-name";

            var arrangeReplicableTemplate = new StatelessServiceHostAspNetCoreListenerReplicaTemplate();

            // Act
            arrangeReplicableTemplate.ConfigureObject(
                c =>
                {
                    c.UseEndpoint(arrangeEndpointName);
                });

            var listener = arrangeReplicableTemplate.Activate(new MockStatelessService());

            // Assert
            Assert.Same(arrangeEndpointName, listener.Name);
        }
    }

    public abstract class ServiceHostAspNetCoreListenerReplicaTemplateTests<TService, TParameters, TConfigurator, TListener>
        where TService : IService
        where TParameters : IServiceHostAspNetCoreListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostAspNetCoreListenerReplicaTemplateConfigurator
    {
    }
}