using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Web
{
    public interface IWebHostExtensionsImpl
    {
        void Run(
            IWebHost @this);
    }
}