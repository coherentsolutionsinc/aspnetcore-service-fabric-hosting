using System;

using Microsoft.ServiceFabric.Services.Remoting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingListenerImplementationTheoryExtension : IUseRemotingListenerImplementationTheoryExtension
    {
        private interface IImplementation : IService
        {
        }

        private sealed class Implementation : IImplementation
        {
        }

        public Func<IServiceProvider, IService> Factory { get; private set; }

        public UseRemotingListenerImplementationTheoryExtension()
        {
            this.Factory = Tools.GetRemotingImplementationFunc<Implementation>();
        }

        public UseRemotingListenerImplementationTheoryExtension Setup<T>()
            where T : IService
        {
            this.Factory = Tools.GetRemotingImplementationFunc<T>();

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