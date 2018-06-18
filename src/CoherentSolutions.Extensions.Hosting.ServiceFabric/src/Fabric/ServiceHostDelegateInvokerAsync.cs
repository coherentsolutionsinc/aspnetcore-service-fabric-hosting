using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostDelegateInvokerAsync : IServiceHostDelegateInvokerAsync
    {
        private readonly IServiceProvider services;

        public ServiceHostDelegateInvokerAsync(
            IServiceProvider services)
        {
            this.services = services;
        }

        public async Task InvokeAsync(Delegate @delegate)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            var arguments = @delegate
               .GetMethodInfo()
               .GetParameters()
               .Select(pi => this.services.GetRequiredService(pi.ParameterType));

            await (Task) @delegate.DynamicInvoke(arguments);
        }
    }
}