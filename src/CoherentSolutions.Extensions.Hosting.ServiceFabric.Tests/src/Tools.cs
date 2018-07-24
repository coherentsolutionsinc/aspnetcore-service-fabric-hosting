using System;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

using Moq;

using ServiceFabric.Mocks;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;
using StatelessService = CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.StatelessService;

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

        public static Func<IStatefulServiceRuntimeRegistrant> GetStatefulRuntimeRegistrantFunc()
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
                        try
                        {
                            var service = f(StatefulContext);
                            var partition = new Mock<IStatefulServicePartition>();

                            Injector.InjectProperty(service, "Partition", partition.Object, true);

                            service.InvokeOnOpenAsync(ReplicaOpenMode.New, c).GetAwaiter().GetResult();

                            var listeners = service.InvokeCreateServiceReplicaListeners()
                               .Select(listener => listener.CreateCommunicationListener(StatefulContext))
                               .ToArray();

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
                        }
                        catch (Exception e)
                        {
                            completionSource.SetException(e);
                        }
                    })
               .Returns(completionSource.Task);

            return () => registrant.Object;
        }

        public static Func<IStatelessServiceRuntimeRegistrant> GetStatelessRuntimeRegistrantFunc()
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
                        try
                        {
                            var service = f(StatelessContext);
                            var partition = new Mock<IStatelessServicePartition>();

                            Injector.InjectProperty(service, "Partition", partition.Object, true);

                            var listeners = service.InvokeCreateServiceInstanceListeners()
                               .Select(listener => listener.CreateCommunicationListener(StatelessContext))
                               .ToArray();

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
                        }
                        catch (Exception e)
                        {
                            completionSource.SetException(e);
                        }
                    })
               .Returns(completionSource.Task);

            return () => registrant.Object;
        }

        public static Func<IServiceCollection> GetDependenciesFunc()
        {
            return () => new ServiceCollection();
        }

        public static Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker> GetDelegateInvokerFunc()
        {
            return (
                @delegate,
                provider) =>
            {
                var invoker = new Mock<IServiceHostDelegateInvoker>();

                invoker
                   .Setup(instance => instance.InvokeAsync(It.IsAny<CancellationToken>()))
                   .Callback<CancellationToken>(
                        cancellationToken =>
                        {
                            @delegate.DynamicInvoke();
                        })
                   .Returns(Task.CompletedTask);

                return invoker.Object;
            };
        }

        public static ServiceHostAspNetCoreCommunicationListenerFactory GetAspNetCoreCommunicationListenerFunc()
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

        public static Func<IWebHostBuilder> GetWebHostBuilderFunc()
        {
            return () =>
            {
                return WebHost.CreateDefaultBuilder();
                //var host = new Mock<IWebHost>();
                //var builder = new Mock<IWebHostBuilder>();
                //var collection = new ServiceCollection();

                //host
                //   .Setup(instance => instance.Services)
                //   .Returns(() => collection.BuildServiceProvider());

                //builder
                //   .Setup(instance => instance.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
                //   .Callback<Action<IServiceCollection>>(
                //        action =>
                //        {
                //            action(collection);
                //        })
                //   .Returns(builder.Object);

                //builder
                //   .Setup(instance => instance.Build())
                //   .Returns(host.Object);

                //return builder.Object;
            };
        }

        public static ServiceHostRemotingCommunicationListenerFactory GetRemotingCommunicationListenerFunc()
        {
            return (
                context,
                build) =>
            {
                var options = build(context);
                var listener = new Mock<FabricTransportServiceRemotingListener>(
                    context,
                    options.MessageHandler,
                    options.ListenerSettings,
                    options.MessageSerializationProvider);

                listener
                   .As<ICommunicationListener>()
                   .Setup(instance => instance.OpenAsync(It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(string.Empty));

                return listener.Object;
            };
        }

        public static Func<IServiceProvider, IService> GetRemotingImplementationFunc<T>()
            where T : IService
        {
            return provider => ActivatorUtilities.CreateInstance<T>(provider);
        }

        public static Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> GetRemotingSerializerFunc<T>()
            where T : IServiceRemotingMessageSerializationProvider
        {
            return provider => ActivatorUtilities.CreateInstance<T>(provider);
        }

        public static Func<IServiceProvider, IServiceRemotingMessageHandler> GetRemotingHandlerFunc<T>()
            where T : IServiceRemotingMessageHandler
        {
            return provider => ActivatorUtilities.CreateInstance<T>(provider);
        }

        public static Func<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator> GetStatefulListenerReplicatorFunc()
        {
            return template =>
            {
                var replicator = new Mock<IStatefulServiceHostListenerReplicator>();
                return replicator.Object;
            };
        }

        public static Func<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator> GetStatelessListenerReplicatorFunc()
        {
            return template =>
            {
                var replicator = new Mock<IStatelessServiceHostListenerReplicator>();
                return replicator.Object;
            };
        }
    }
}