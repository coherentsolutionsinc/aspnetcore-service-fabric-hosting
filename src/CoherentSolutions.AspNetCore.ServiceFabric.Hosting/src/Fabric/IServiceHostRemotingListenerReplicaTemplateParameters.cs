using System;

using Microsoft.ServiceFabric.Services.Remoting.V2;

using IRemotingImplementation = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostRemotingListenerReplicaTemplateParameters
        : IServiceHostListenerReplicaTemplateParameters
    {
        Func<IServiceProvider, IRemotingImplementation> RemotingImplementationFunc { get; }

        Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> RemotingSerializerFunc { get; }
    }
}