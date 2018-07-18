using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools
{
    internal class OpenGenericAwareStartup : IStartup
    {
        private readonly IStartup impl;

        public OpenGenericAwareStartup(
            IStartup impl)
        {
            this.impl = impl;
        }

        public IServiceProvider ConfigureServices(
            IServiceCollection services)
        {
            return new OpenGenericAwareServiceProvider(this.impl.ConfigureServices(services));
        }

        public void Configure(
            IApplicationBuilder app)
        {
            this.impl.Configure(app);
        }
    }
}