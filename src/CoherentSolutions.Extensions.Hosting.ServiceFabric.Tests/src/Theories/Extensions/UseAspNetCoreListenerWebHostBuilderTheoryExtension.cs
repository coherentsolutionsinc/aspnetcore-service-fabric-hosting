using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseAspNetCoreListenerWebHostBuilderTheoryExtension : IUseAspNetCoreListenerWebHostBuilderTheoryExtension
    {
        public Func<IWebHostBuilder> Factory { get; private set; }

        public UseAspNetCoreListenerWebHostBuilderTheoryExtension()
        {
            this.Factory = () => new MockWebHostBuilder();
        }

        public UseAspNetCoreListenerWebHostBuilderTheoryExtension Setup(
            Func<IWebHostBuilder> factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}