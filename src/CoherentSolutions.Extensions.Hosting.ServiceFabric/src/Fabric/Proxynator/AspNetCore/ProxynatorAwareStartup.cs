﻿using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.AspNetCore
{
    internal class ProxynatorAwareStartup : IStartup
    {
        private readonly IStartup impl;

        public ProxynatorAwareStartup(
            IStartup impl)
        {
            this.impl = impl;
        }

        public IServiceProvider ConfigureServices(
            IServiceCollection services)
        {
            return new ProxynatorAwareServiceProvider(this.impl.ConfigureServices(services));
        }

        public void Configure(
            IApplicationBuilder app)
        {
            this.impl.Configure(app);
        }
    }
}