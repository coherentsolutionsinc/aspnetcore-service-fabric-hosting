using System;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class LocalRuntimeRemotingListenerReplicaTemplate
        : ConfigurableObject<IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator>,
          IStatelessServiceHostRemotingListenerReplicaTemplate
    {
        private class CommunicationListener : ICommunicationListener
        {
            public Task<string> OpenAsync(
                CancellationToken cancellationToken)
            {
                return Task.FromResult("Ignored (remoting isn't supported)");
            }

            public Task CloseAsync(
                CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public void Abort()
            {
            }
        }

        private class Parameters
            : IStatelessServiceHostRemotingListenerReplicaTemplateParameters,
              IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator
        {
            public string EndpointName { get; private set; }

            public Func<IServiceCollection> DependenciesFunc { get; }

            public Action<IServiceCollection> DependenciesConfigAction { get; }

            public Func<IConfigurableObjectLoggerOptions> LoggerOptionsFunc { get; }

            public ServiceHostRemotingCommunicationListenerFactory RemotingCommunicationListenerFunc { get; }

            public Func<IServiceProvider, Microsoft.ServiceFabric.Services.Remoting.IService> RemotingImplementationFunc { get; }

            public Func<FabricTransportRemotingListenerSettings> RemotingSettingsFunc { get; }

            public Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> RemotingSerializationProviderFunc { get; }

            public Func<IServiceProvider, IServiceRemotingMessageHandler> RemotingHandlerFunc { get; }

            public void UseEndpoint(
                string endpointName)
            {
                this.EndpointName = endpointName
                 ?? throw new ArgumentNullException(nameof(endpointName));
            }

            public void UseDependencies(
                Func<IServiceCollection> factoryFunc)
            {
            }

            public void ConfigureDependencies(
                Action<IServiceCollection> configAction)
            {
            }

            public void UseLoggerOptions(
                Func<IConfigurableObjectLoggerOptions> factoryFunc)
            {
            }

            public void UseCommunicationListener(
                ServiceHostRemotingCommunicationListenerFactory factoryFunc)
            {
            }

            public void UseImplementation<TImplementation>(
                Func<IServiceProvider, TImplementation> factoryFunc)
                where TImplementation : Microsoft.ServiceFabric.Services.Remoting.IService
            {
            }

            public void UseSettings(
                Func<FabricTransportRemotingListenerSettings> factoryFunc)
            {
            }

            public void UseSerializationProvider<TSerializationProvider>(
                Func<IServiceProvider, TSerializationProvider> factoryFunc)
                where TSerializationProvider : IServiceRemotingMessageSerializationProvider
            {
            }

            public void UseHandler<THandler>(
                Func<IServiceProvider, THandler> factoryFunc)
                where THandler : IServiceRemotingMessageHandler
            {
            }
        }

        public ServiceInstanceListener Activate(
            IStatelessService service)
        {
            var parameters = new Parameters();

            this.UpstreamConfiguration(parameters);

            return new ServiceInstanceListener(context => new CommunicationListener(), parameters.EndpointName);
        }
    }
}