using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingListenerHandlerTheoryExtension : IUseRemotingListenerHandlerTheoryExtension
    {
        public Func<IServiceProvider, IServiceRemotingMessageHandler> Factory { get; private set; }

        public UseRemotingListenerHandlerTheoryExtension()
        {
            this.Factory = provider => new MockServiceRemotingMessageHandler();
        }

        public UseRemotingListenerHandlerTheoryExtension Setup<T>()
            where T : IServiceRemotingMessageHandler
        {
            this.Factory = provider => ActivatorUtilities.CreateInstance<T>(provider);

            return this;
        }

        public UseRemotingListenerHandlerTheoryExtension Setup(
            Func<IServiceProvider, IServiceRemotingMessageHandler> factory)
        {
            this.Factory = factory
             ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}