using System;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Threading;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

using Moq;

using ServiceFabric.Mocks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
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

        private const string LOCALHOST = "localhost";

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

        public static Func<
            ServiceContext,
            string,
            Func<string, AspNetCoreCommunicationListener, IWebHost>,
            AspNetCoreCommunicationListener
        > StatefulAspNetCoreCommunicationListenerFunc
        {
            get
            {
                return (
                    context,
                    s,
                    arg3) =>
                {
                    var action = new Mock<Func<string, AspNetCoreCommunicationListener, IWebHost>>();
                    var listener = new Mock<AspNetCoreCommunicationListener>(StatefulContext, action.Object);

                    listener
                       .Setup(instance => instance.OpenAsync(It.IsAny<CancellationToken>()))
                       .Callback(() => arg3("localhost", listener.Object));

                    return listener.Object;
                };
            }
        }

        public static Func<
            ServiceContext,
            string,
            Func<string, AspNetCoreCommunicationListener, IWebHost>,
            AspNetCoreCommunicationListener
        > StatelessAspNetCoreCommunicationListenerFunc
        {
            get
            {
                return (
                    context,
                    s,
                    arg3) =>
                {
                    var action = new Mock<Func<string, AspNetCoreCommunicationListener, IWebHost>>();
                    var listener = new Mock<AspNetCoreCommunicationListener>(StatefulContext, action.Object);

                    listener
                       .Setup(instance => instance.OpenAsync(It.IsAny<CancellationToken>()))
                       .Callback(() => arg3(LOCALHOST, listener.Object));

                    return listener.Object;
                };
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