using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions
{
    public class UseAspNetCoreListenerWebHostBuilderTheoryExtension : IUseAspNetCoreListenerWebHostBuilderTheoryExtension
    {
        private sealed class Startup : IStartup
        {
            public IServiceProvider ConfigureServices(
                IServiceCollection services)
            {
                return new DefaultServiceProviderFactory().CreateServiceProvider(services);
            }

            public void Configure(
                IApplicationBuilder app)
            {
            }
        }

        public Func<IWebHostBuilder> Factory { get; private set; }

        public UseAspNetCoreListenerWebHostBuilderTheoryExtension()
        {
            this.Factory = Tools.GetWebHostBuilderFunc();
        }

        public UseAspNetCoreListenerWebHostBuilderTheoryExtension Setup(
            Func<IWebHostBuilder> factory)
        {
            this.Factory = factory;

            return this;
        }
    }
}