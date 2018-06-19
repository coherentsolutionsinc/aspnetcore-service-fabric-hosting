using System;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAsyncDelegate
    {
        Task InvokeAsync();
    }
}