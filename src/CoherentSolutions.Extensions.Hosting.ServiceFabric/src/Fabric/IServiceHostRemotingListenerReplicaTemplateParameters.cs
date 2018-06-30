using System;

using Microsoft.ServiceFabric.Services.Remoting.V2;

using IRemotingImplementation = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostRemotingListenerReplicaTemplateParameters
        : IServiceHostListenerReplicaTemplateParameters
    {
        ServiceHostRemotingCommunicationListenerFactory RemotingCommunicationListenerFunc { get; }

        Func<IServiceProvider, IRemotingImplementation> RemotingImplementationFunc { get; }

        Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> RemotingSerializerFunc { get; }
    }
}