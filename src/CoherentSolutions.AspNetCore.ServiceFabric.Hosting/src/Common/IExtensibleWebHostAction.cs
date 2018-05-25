using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common
{
    public interface IExtensibleWebHostAction
    {
        void Execute(
            IWebHost webHost);
    }
}