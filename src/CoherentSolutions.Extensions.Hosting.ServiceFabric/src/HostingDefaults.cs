using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;

using IRemotingImplementation = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric
{
    public static class HostingDefaults
    {
        public static AspNetCoreCommunicationListener DefaultAspNetCoreCommunicationListenerFunc(
            ServiceContext serviceContext,
            string endpointName,
            Func<string, AspNetCoreCommunicationListener, IWebHost> build)
        {
            var @delegate = new Func<string, AspNetCoreCommunicationListener, IWebHost>(build);
            return new KestrelCommunicationListener(serviceContext, endpointName, @delegate);
        }

        public static IWebHostBuilder DefaultWebHostBuilderFunc()
        {
            return WebHost.CreateDefaultBuilder();
        }

        public static void DefaultWebHostConfigAction(
            IWebHostBuilder builder)
        {
        }

        public static FabricTransportServiceRemotingListener DefaultRemotingCommunicationListenerFunc(
            ServiceContext serviceContext,
            ServiceHostRemotingCommunicationListenerComponentsFactory build)
        {
            var components = build(serviceContext);
            return new FabricTransportServiceRemotingListener(
                serviceContext,
                components.MessageDispatcher,
                components.ListenerSettings,
                components.MessageSerializationProvider);
        }

        public static IRemotingImplementation DefaultRemotingImplementationFunc<T>(
            IServiceProvider provider)
        where T : IRemotingImplementation
        {
            return ActivatorUtilities.CreateInstance<T>(provider);
        }

        public static FabricTransportRemotingListenerSettings DefaultRemotingSettingsFunc()
        {
            return new FabricTransportRemotingListenerSettings();
        }

        public static IServiceRemotingMessageSerializationProvider DefaultRemotingSerializerFunc<T>(
            IServiceProvider provider)
            where T : IServiceRemotingMessageSerializationProvider
        {
            return ActivatorUtilities.CreateInstance<T>(provider);
        }

        public static IServiceHostDelegateInvoker DefaulDelegateInvokerFunc(
            Delegate @delegate,
            IServiceProvider services)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return new ServiceHostDelegateInvoker(@delegate, services);
        }

        public static IServiceHostLoggerOptions DefaultLoggerOptionsFunc()
        {
            return ServiceHostLoggerOptions.Disabled;
        }

        public static IServiceCollection DefaultDependenciesFunc()
        {
            return new ServiceCollection();
        }
    }
}