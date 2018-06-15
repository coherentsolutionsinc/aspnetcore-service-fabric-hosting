using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Web
{
    public class WebHostBuilderExtensionsImpl : IWebHostBuilderExtensionsImpl
    {
        public void UseServiceFabricIntegration(
            IWebHostBuilder @this,
            AspNetCoreCommunicationListener listener,
            ServiceFabricIntegrationOptions options)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            @this.UseServiceFabricIntegration(listener, options);
        }

        public void UseUrls(
            IWebHostBuilder @this,
            params string[] urls)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            @this.UseUrls(urls);
        }
    }
}