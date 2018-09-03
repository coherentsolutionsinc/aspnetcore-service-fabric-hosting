using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingListenerImplementationTheoryExtension : IUseRemotingListenerImplementationTheoryExtension
    {
        public Func<IServiceProvider, IService> Factory { get; private set; }

        public UseRemotingListenerImplementationTheoryExtension()
        {
            this.Factory = provider => new MockServiceRemotingListenerImplementation();
        }

        public UseRemotingListenerImplementationTheoryExtension Setup<T>()
            where T : IService
        {
            this.Factory = provider => ActivatorUtilities.CreateInstance<T>(provider);

            return this;
        }

        public UseRemotingListenerImplementationTheoryExtension Setup(
            Func<IServiceProvider, IService> factory)
        {
            this.Factory = factory
             ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}