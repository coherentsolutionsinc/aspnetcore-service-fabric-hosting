using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class ServiceHostDelegateInvoker : IServiceHostDelegateInvoker
    {
        private readonly IServiceProvider services;

        private readonly Func<IServiceProvider, CancellationToken, Task> invocation;

        public ServiceHostDelegateInvoker(
            Delegate @delegate,
            IServiceProvider services)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            this.services = services
             ?? throw new ArgumentNullException(nameof(services));

            this.invocation =
                (
                    dependencies,
                    cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var arguments = @delegate
                       .GetMethodInfo()
                       .GetParameters()
                       .Select(
                            (
                                pi,
                                index) =>
                            {
                                if (pi.ParameterType == typeof(CancellationToken))
                                {
                                    return cancellationToken;
                                }

                                return dependencies.GetRequiredService(pi.ParameterType);
                            })
                       .ToArray();

                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var result = @delegate.DynamicInvoke(arguments);
                        if (result is Task returnTask)
                        {
                            return returnTask;
                        }

                        return Task.CompletedTask;
                    }
                    catch (TargetInvocationException e)
                    {
                        if (e.InnerException != null)
                        {
                            ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                        }

                        throw;
                    }
                };
        }

        public Task InvokeAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.invocation(this.services, cancellationToken);
        }
    }
}