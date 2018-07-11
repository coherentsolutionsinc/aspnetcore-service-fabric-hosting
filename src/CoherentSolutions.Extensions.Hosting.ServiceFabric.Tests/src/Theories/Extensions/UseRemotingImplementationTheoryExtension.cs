using System;

using Microsoft.ServiceFabric.Services.Remoting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseRemotingImplementationTheoryExtension : IUseRemotingImplementationTheoryExtension
    {
        private interface IRemotingImplementation : IService
        {
        }

        private sealed class RemotingImplementation : IRemotingImplementation
        {
        }

        public Func<IServiceProvider, IService> Factory { get; private set; }

        public UseRemotingImplementationTheoryExtension()
        {
            this.Factory = Tools.GetRemotingImplementationFunc<RemotingImplementation>();
        }

        public UseRemotingImplementationTheoryExtension Setup<T>()
            where T : IService
        {
            this.Factory = Tools.GetRemotingImplementationFunc<T>();

            return this;
        }

        public UseRemotingImplementationTheoryExtension Setup(
            Func<IServiceProvider, IService> factory)
        {
            this.Factory = factory
             ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}