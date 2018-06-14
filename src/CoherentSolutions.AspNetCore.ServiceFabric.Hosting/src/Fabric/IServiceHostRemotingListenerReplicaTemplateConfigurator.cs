using System;

using Microsoft.ServiceFabric.Services.Remoting.V2;

using IRemotingImplementation = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostRemotingListenerReplicaTemplateConfigurator
        : IServiceHostListenerReplicaTemplateConfigurator
    {
        void UseImplementation<TImplementation>(
            Func<TImplementation> factoryFunc)
            where TImplementation : IRemotingImplementation;

        void UseSerializer<TSerializer>(
            Func<TSerializer> factoryFunc)
            where TSerializer : IServiceRemotingMessageSerializationProvider;
    }
}