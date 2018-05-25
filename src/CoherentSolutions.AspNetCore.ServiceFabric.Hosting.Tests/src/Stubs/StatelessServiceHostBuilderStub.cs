using System;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs
{
    internal class StatelessServiceHostBuilderStub : IStatelessServiceHostBuilder
    {
        private class StatelessServiceHostStub : IStatelessServiceHost
        {
            public void Run()
            {
            }
        }

        public void ConfigureObject(
            Action<IStatelessServiceHostBuilderConfigurator> configAction)
        {
        }

        public IStatelessServiceHost Build()
        {
            return new StatelessServiceHostStub();
        }
    }
}