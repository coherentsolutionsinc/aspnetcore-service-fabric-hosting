using System;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs
{
    internal class StatefulServiceHostBuilderStub : IStatefulServiceHostBuilder
    {
        private class StatefulServiceHostStub : IStatefulServiceHost
        {
            public void Run()
            {
            }
        }

        public void ConfigureObject(
            Action<IStatefulServiceHostBuilderConfigurator> configAction)
        {
        }

        public IStatefulServiceHost Build()
        {
            return new StatefulServiceHostStub();
        }
    }
}