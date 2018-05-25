using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web
{
    public interface IWebHostBuilderExtensionsImpl
    {
        void UseServiceFabricIntegration(
            IWebHostBuilder @this,
            AspNetCoreCommunicationListener listener,
            ServiceFabricIntegrationOptions options);

        void UseUrls(
            IWebHostBuilder @this,
            params string[] urls);
    }
}