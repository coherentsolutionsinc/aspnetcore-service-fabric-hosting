using System;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateInvokerAsync
    {
        Task InvokeAsync(Delegate @delegate);
    }
}