using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseEventSourceImplementationTheoryExtension : IUseEventSourceImplementationTheoryExtension
    {
        public Func<IServiceProvider, IServiceEventSource> Factory { get; private set; }

        public UseEventSourceImplementationTheoryExtension()
        {
            this.Factory = provider => new MockServiceEventSource();
        }

        public UseEventSourceImplementationTheoryExtension Setup<T>()
            where T : IServiceEventSource
        {
            this.Factory = provider => ActivatorUtilities.CreateInstance<T>(provider);

            return this;
        }

        public UseEventSourceImplementationTheoryExtension Setup(
            Func<IServiceProvider, IServiceEventSource> factory)
        {
            this.Factory = factory
             ?? throw new ArgumentNullException(nameof(factory));

            return this;
        }
    }
}