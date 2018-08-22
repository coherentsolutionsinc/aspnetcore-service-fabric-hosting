using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostDelegateInvoker
    {
        private readonly Delegate @delegate;

        protected ServiceHostDelegateInvoker(
            Delegate @delegate)
        {
            this.@delegate = @delegate
             ?? throw new ArgumentNullException(nameof(@delegate));
        }

        public Func<CancellationToken, Task> CreateInvocation(
            IServiceProvider invocationArgumentsProvider)
        {
            Task Function(
                CancellationToken cancellationToken)
            {
                var argumentsProvider = new ReplaceAwareServiceProvider(
                    new Dictionary<Type, object>
                    {
                        [typeof(CancellationToken)] = cancellationToken
                    },
                    invocationArgumentsProvider);

                var arguments = this.@delegate.GetMethodInfo()
                   .GetParameters()
                   .Select(
                        (
                            pi,
                            index) => argumentsProvider.GetRequiredService(pi.ParameterType))
                   .ToArray();

                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var result = this.@delegate.DynamicInvoke(arguments);
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
            }

            return Function;
        }
    }
}