using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web
{
    public interface IWebHostExtensionsImpl
    {
        void Run(
            IWebHost @this);
    }
}