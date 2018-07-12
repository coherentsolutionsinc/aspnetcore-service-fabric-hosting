using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

using IRemotingImplementation = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostRemotingListenerReplicaTemplateConfigurator
        : IServiceHostListenerReplicaTemplateConfigurator,
          IConfigurableObjectDependenciesConfigurator
    {
        void UseCommunicationListener(
            ServiceHostRemotingCommunicationListenerFactory factoryFunc);

        void UseImplementation<TImplementation>(
            Func<IServiceProvider, TImplementation> factoryFunc)
            where TImplementation : IRemotingImplementation;

        void UseSettings(
            Func<FabricTransportRemotingListenerSettings> factoryFunc);

        void UseSerializationProvider<TSerializer>(
            Func<IServiceProvider, TSerializer> factoryFunc)
            where TSerializer : IServiceRemotingMessageSerializationProvider;

        void UseHandler<THandler>(
            Func<IServiceProvider, THandler> factoryFunc)
            where THandler : IServiceRemotingMessageHandler;
    }
}