using System;

using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseRemotingSerializerTheoryExtension : ITheoryExtension
    {
        Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> Factory { get; }
    }
}