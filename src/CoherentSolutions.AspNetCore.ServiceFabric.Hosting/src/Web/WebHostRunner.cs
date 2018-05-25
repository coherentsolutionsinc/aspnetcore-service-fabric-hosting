using System;
using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web
{
    public class WebHostRunner : IWebHostRunner
    {
        private readonly IWebHost host;

        private readonly IWebHostExtensionsImpl impl;

        public WebHostRunner(
            IWebHost host,
            IWebHostExtensionsImpl extensionsProxy)
        {
            this.host = host
             ?? throw new ArgumentNullException(nameof(host));

            this.impl = extensionsProxy
             ?? throw new ArgumentNullException(nameof(extensionsProxy));
        }

        public void Run()
        {
            this.impl.Run(this.host);
        }
    }
}