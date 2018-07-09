using System;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

using Moq;

using ServiceFabric.Mocks;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;
using StatelessService = CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.StatelessService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests
{
    public static class Tools
    {
        public interface ITestDependency
        {
        }

        public interface ITestRemoting : IService
        {
        }

        public class TestRemoting : ITestRemoting
        {
        }

        public class TestDependency : ITestDependency
        {
        }

        private class EndpointResourceDescriptionCollection : KeyedCollection<string, EndpointResourceDescription>
        {
            protected override string GetKeyForItem(
                EndpointResourceDescription item)
            {
                return item.Name;
            }
        }

        private const string LOCALHOST = "localhost";

        private const string ENDPOINT_NAME = "ServiceEndpoint";

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

        public static AspNetCoreCommunicationListener DefaultStatefulAspNetCoreCommunicationListener
        {
            get
            {
                return AspNetCoreCommunicationListenerFunc(
                    StatefulContext,
                    ENDPOINT_NAME,
                    (
                        s,
                        listener) => null);
            }
        }

        public static AspNetCoreCommunicationListener DefaultStatelessAspNetCoreCommunicationListener
        {
            get
            {
                return AspNetCoreCommunicationListenerFunc(
                    StatelessContext,
                    ENDPOINT_NAME,
                    (
                        s,
                        listener) => null);
            }
        }

        public static Func<IStatefulServiceRuntimeRegistrant> StatefulRuntimeRegistrant
        {
            get
            {
                var completionSource = new TaskCompletionSource<int>();
                var registrant = new Mock<IStatefulServiceRuntimeRegistrant>();
                registrant
                   .Setup(
                        instance => instance.RegisterAsync(
                            It.IsAny<string>(),
                            It.IsAny<Func<StatefulServiceContext, StatefulServiceBase>>(),
                            It.IsAny<CancellationToken>()))
                   .Callback<string, Func<StatefulServiceContext, StatefulServiceBase>, CancellationToken>(
                        (
                            s,
                            f,
                            c) =>
                        {
                            var service = f(StatefulContext);
                            var partition = new Mock<IStatefulServicePartition>();

                            Injector.InjectProperty(service, "Partition", partition.Object, true);

                            service.InvokeOnOpenAsync(ReplicaOpenMode.New, c).GetAwaiter().GetResult();

                            var listeners = service.InvokeCreateServiceReplicaListeners()
                               .Select(listener => listener.CreateCommunicationListener(StatefulContext));

                            var openListenersTask = Task.Run(
                                () =>
                                {
                                    foreach (var listener in listeners)
                                    {
                                        listener.OpenAsync(c).GetAwaiter().GetResult();
                                    }
                                });
                            var runTask = service.InvokeRunAsync(c);

                            openListenersTask.GetAwaiter().GetResult();

                            var changeRoleTask = service.InvokeOnChangeRoleAsync(ReplicaRole.Primary, c);

                            Task.WhenAll(runTask, changeRoleTask).GetAwaiter().GetResult();

                            completionSource.SetResult(0);
                        })
                   .Returns(completionSource.Task);

                return () => registrant.Object;
            }
        }

        public static Func<IStatelessServiceRuntimeRegistrant> StatelessRuntimeRegistrant
        {
            get
            {
                var completionSource = new TaskCompletionSource<int>();
                var registrant = new Mock<IStatelessServiceRuntimeRegistrant>();
                registrant
                   .Setup(
                        instance => instance.RegisterAsync(
                            It.IsAny<string>(),
                            It.IsAny<Func<StatelessServiceContext, StatelessService>>(),
                            It.IsAny<CancellationToken>()))
                   .Callback<string, Func<StatelessServiceContext, StatelessService>, CancellationToken>(
                        (
                            s,
                            f,
                            c) =>
                        {
                            var service = f(StatelessContext);
                            var partition = new Mock<IStatelessServicePartition>();

                            Injector.InjectProperty(service, "Partition", partition.Object, true);

                            var listeners = service.InvokeCreateServiceInstanceListeners()
                               .Select(listener => listener.CreateCommunicationListener(StatelessContext));

                            Task.WhenAll(
                                    Task.Run(
                                        () =>
                                        {
                                            foreach (var listener in listeners)
                                            {
                                                listener.OpenAsync(c).GetAwaiter().GetResult();
                                            }
                                        }),
                                    service.InvokeRunAsync(c))
                               .GetAwaiter()
                               .GetResult();

                            service.InvokeOnOpenAsync(c).GetAwaiter().GetResult();

                            completionSource.SetResult(0);
                        })
                   .Returns(completionSource.Task);

                return () => registrant.Object;
            }
        }

        public static ServiceHostAspNetCoreCommunicationListenerFactory AspNetCoreCommunicationListenerFunc
        {
            get
            {
                return (
                    context,
                    s,
                    arg3) =>
                {
                    var action = new Mock<Func<string, AspNetCoreCommunicationListener, IWebHost>>();
                    var listener = new Mock<AspNetCoreCommunicationListener>(context, action.Object);

                    listener
                       .Setup(instance => instance.OpenAsync(It.IsAny<CancellationToken>()))
                       .Callback(() => arg3(LOCALHOST, listener.Object))
                       .Returns(Task.FromResult(string.Empty));

                    return listener.Object;
                };
            }
        }

        public static ServiceHostRemotingCommunicationListenerFactory RemotingCommunicationListenerFunc
        {
            get
            {
                return (
                    context,
                    build) =>
                {
                    var options = build(context);
                    var listener = new Mock<FabricTransportServiceRemotingListener>(
                        context,
                        options.MessageDispatcher,
                        options.ListenerSettings,
                        options.MessageSerializationProvider);

                    listener
                       .As<ICommunicationListener>()
                       .Setup(instance => instance.OpenAsync(It.IsAny<CancellationToken>()))
                       .Returns(Task.FromResult(string.Empty));

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