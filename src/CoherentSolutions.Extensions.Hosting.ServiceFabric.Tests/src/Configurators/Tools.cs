using System;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.ServiceFabric.Data;

using Moq;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Configurators
{
    public static class Tools
    {
        private class EndpointResourceDescriptionCollection : KeyedCollection<string, EndpointResourceDescription>
        {
            protected override string GetKeyForItem(
                EndpointResourceDescription item)
            {
                return item.Name;
            }
        }

        private static readonly Mock<ICodePackageActivationContext> package;

        public static StatefulServiceContext StatefulContext
            => MockStatefulServiceContextFactory.Create(
                package.Object,
                MockStatefulServiceContextFactory.ServiceTypeName,
                new Uri(MockStatefulServiceContextFactory.ServiceName),
                Guid.Empty,
                default(long));

        public static StatelessServiceContext StatelessContext
            => MockStatelessServiceContextFactory.Create(
                package.Object,
                MockStatelessServiceContextFactory.ServiceTypeName,
                new Uri(MockStatelessServiceContextFactory.ServiceName),
                Guid.Empty,
                default(long));

        public static IStatefulService StatefulService
        {
            get
            {
                var service = new Mock<IStatefulService>();
                service.Setup(instance => instance.GetContext()).Returns(MockStatefulServiceContextFactory.Default);
                service.Setup(instance => instance.GetPartition()).Returns(new Mock<IStatefulServicePartition>().Object);
                service.Setup(instance => instance.GetEventSource()).Returns(new Mock<IServiceEventSource>().Object);
                service.Setup(instance => instance.GetReliableStateManager()).Returns(new Mock<IReliableStateManager>().Object);
                return service.Object;
            }
        }

        public static IStatelessService StatelessService
        {
            get
            {
                var service = new Mock<IStatelessService>();
                service.Setup(instance => instance.GetContext()).Returns(MockStatelessServiceContextFactory.Default);
                service.Setup(instance => instance.GetPartition()).Returns(new Mock<IStatelessServicePartition>().Object);
                service.Setup(instance => instance.GetEventSource()).Returns(new Mock<IServiceEventSource>().Object);
                return service.Object;
            }
        }

        static Tools()
        {
            package = new Mock<ICodePackageActivationContext>();
            package
               .Setup(instance => instance.GetEndpoints())
               .Returns(
                    new EndpointResourceDescriptionCollection
                    {
                        new EndpointResourceDescription
                        {
                            Name = "ServiceEndpoint"
                        }
                    });
        }
    }
}