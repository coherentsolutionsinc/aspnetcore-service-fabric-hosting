using System;
using System.Fabric.Management.ServiceModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostAsyncDelegate
        : IServiceHostAsyncDelegate
    {
        private readonly Delegate @delegate;

        private readonly object[] arguments;

        public ServiceHostAsyncDelegate(
            Delegate @delegate,
            IServiceProvider services)
        {
            this.@delegate = @delegate 
             ?? throw new ArgumentNullException(nameof(@delegate));
            
            this.arguments = this.@delegate
               .GetMethodInfo()
               .GetParameters()
               .Select(pi => services.GetRequiredService(pi.ParameterType))
               .ToArray();

            var mi = @delegate.GetMethodInfo();
            if (typeof(Task) != mi.ReturnType)
            {
                throw new ArgumentException($"The return type of delegate should be {typeof(Task)}");
            }
        }

        public async Task InvokeAsync()
        {
            await (Task) this.@delegate.DynamicInvoke(this.arguments);
        }
    }
}