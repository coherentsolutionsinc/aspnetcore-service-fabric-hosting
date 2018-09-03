using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Remoting.V2;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingListenerSerializationProviderTheoryExtension : IUseRemotingListenerSerializerTheoryExtension
    {
        public Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> Factory { get; private set; }

        public UseRemotingListenerSerializationProviderTheoryExtension()
        {
            this.Factory = provider => new MockServiceRemotingMessageSerializationProvider();
        }

        public UseRemotingListenerSerializationProviderTheoryExtension Setup<T>()
            where T : IServiceRemotingMessageSerializationProvider
        {
            this.Factory = provider => ActivatorUtilities.CreateInstance<T>(provider);

            return this;
        }

        public UseRemotingListenerSerializationProviderTheoryExtension Setup(
            Func<IServiceProvider, IServiceRemotingMessageSerializationProvider> factory)
        {
            this.Factory = factory
             ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}