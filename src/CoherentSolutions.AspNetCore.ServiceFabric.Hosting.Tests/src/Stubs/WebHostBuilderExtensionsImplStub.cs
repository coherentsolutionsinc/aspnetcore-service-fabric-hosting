using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests.Stubs
{
    internal class WebHostBuilderExtensionsImplStub : IWebHostBuilderExtensionsImpl
    {
        public void UseServiceFabricIntegration(
            IWebHostBuilder @this,
            AspNetCoreCommunicationListener listener,
            ServiceFabricIntegrationOptions options)
        {
        }

        public void UseUrls(
            IWebHostBuilder @this,
            params string[] urls)
        {
        }

        public static IWebHostBuilderExtensionsImpl Func()
        {
            return new WebHostBuilderExtensionsImplStub();
        }
    }
}