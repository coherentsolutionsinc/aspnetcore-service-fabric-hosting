using System;

using Microsoft.ServiceFabric.Services.Remoting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public interface IUseRemotingImplementationTheoryExtension : ITheoryExtension
    {
        Func<IServiceProvider, IService> Factory { get; }
    }
}