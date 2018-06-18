using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostDelegateAsync
    {
        public Delegate Delegate { get; }

        public ServiceHostDelegateAsync(
            Delegate @delegate)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            var mi = @delegate.GetMethodInfo();
            if (typeof(Task) != mi.ReturnType)
            {
                throw new ArgumentException($"The return type of delegate should be {typeof(Task)}");
            }
            this.Delegate = @delegate;
        }
    }
}