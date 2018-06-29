using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.ServiceFabric.Services.Remoting.V2;

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
            Func<TImplementation> factoryFunc)
            where TImplementation : IRemotingImplementation;

        void UseSerializer<TSerializer>(
            Func<TSerializer> factoryFunc)
            where TSerializer : IServiceRemotingMessageSerializationProvider;
    }
}