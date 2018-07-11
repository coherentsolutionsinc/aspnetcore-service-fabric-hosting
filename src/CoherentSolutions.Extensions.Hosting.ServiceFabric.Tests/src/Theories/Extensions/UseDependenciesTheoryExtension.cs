using System;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseDependenciesTheoryExtension : IUseDependenciesTheoryExtension
    {
        public Func<IServiceCollection> Factory { get; private set; }

        public UseDependenciesTheoryExtension()
        {
            this.Factory = Tools.GetDependenciesFunc();
        }

        public UseDependenciesTheoryExtension Setup(
            Func<IServiceCollection> factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}