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
            this.Factory = provider => new RemotingImplementation();
        }

        public UseRemotingImplementationTheoryExtension SetupImplementation<T>()
            where T : IService
        {
            this.Factory = HostingDefaults.DefaultRemotingImplementationFunc<T>;

            return this;
        }

        public UseRemotingImplementationTheoryExtension SetupImplementation<T>(
            Func<IServiceProvider, IService> factory)
        {
            this.Factory = factory ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}