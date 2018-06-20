using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostDelegate
        : IServiceHostDelegate
    {
        private readonly Delegate @delegate;

        private readonly object[] arguments;

        private readonly CancellationTokenSource cancellationTokenSource;

        public ServiceLifecycleEvent LifecycleEvent { get; }

        public ServiceHostDelegate(
            Delegate @delegate,
            ServiceLifecycleEvent lifecycleEvent,
            IServiceProvider services)
        {
            this.LifecycleEvent = lifecycleEvent;

            this.@delegate = @delegate
             ?? throw new ArgumentNullException(nameof(@delegate));

            this.cancellationTokenSource = new CancellationTokenSource();

            this.arguments = this.@delegate
               .GetMethodInfo()
               .GetParameters()
               .Select(
                    (
                        pi,
                        index) =>
                    {
                        if (pi.ParameterType == typeof(CancellationToken))
                        {
                            return this.cancellationTokenSource.Token;
                        }

                        return services.GetRequiredService(pi.ParameterType);
                    })
               .ToArray();

            var mi = @delegate.GetMethodInfo();
            if (typeof(Task) != mi.ReturnType)
            {
                throw new ArgumentException($"The return type of delegate should be {typeof(Task)}");
            }
        }

        public async Task InvokeAsync(
            CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(() => this.cancellationTokenSource.Cancel()))
            {
                try
                {
                    await (Task) this.@delegate.DynamicInvoke(this.arguments);
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException != null)
                    {
                        ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                    }
                    throw;
                }
            }
        }
    }
}