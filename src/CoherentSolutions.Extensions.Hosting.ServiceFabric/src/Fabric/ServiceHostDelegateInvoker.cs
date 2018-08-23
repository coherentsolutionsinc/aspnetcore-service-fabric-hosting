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
    public abstract class ServiceHostDelegateInvoker<TInvocationContext>
        : IServiceHostDelegateInvoker<TInvocationContext>
    {
        private readonly Delegate @delegate;

        private readonly IServiceProvider services;

        protected ServiceHostDelegateInvoker(
            Delegate @delegate,
            IServiceProvider services)
        {
            this.@delegate = @delegate
             ?? throw new ArgumentNullException(nameof(@delegate));

            this.services = services 
             ?? throw new ArgumentNullException(nameof(services));
        }

        public Task InvokeAsync(
            TInvocationContext invocationContext,
            CancellationToken cancellationToken)
        {
            if (invocationContext == null)
            {
                throw new ArgumentNullException(nameof(invocationContext));
            }

            var argumentsProvider = new ReplaceAwareServiceProvider(
                new Dictionary<Type, object>
                {
                    [typeof(TInvocationContext)] = invocationContext,
                    [typeof(CancellationToken)] = cancellationToken
                },
                this.services);


            // This is required to support closures (for example generated using Linq.Expression) not delegates.
            var parameters = this.@delegate.GetType().DeclaringType == null
                ? this.@delegate.GetMethodInfo().GetParameters().Skip(1)
                : this.@delegate.GetMethodInfo().GetParameters();

            var arguments = parameters
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
    }
}