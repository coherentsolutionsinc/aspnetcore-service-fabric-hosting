using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.DependencyInjection;

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

            // Giving custom invokers a chance to register context payload as first class injections
            var replacements = new Dictionary<Type, object>
            {
                [typeof(TInvocationContext)] = invocationContext,
                [typeof(CancellationToken)] = cancellationToken
            };

            foreach (var tuple in this.UnwrapInvocationContext(invocationContext))
            {
                replacements[tuple.t] = tuple.o;
            }

            var argumentsProvider = new ReplaceAwareServiceProvider(replacements, this.services);

            // This is required to support closures (for example generated using Linq.Expression) not delegates.

            var method = this.@delegate.GetMethodInfo();
            var parameters = method.GetParameters();

            if (method.IsStatic)
            {
                if (this.@delegate.Target != null)
                {
                    parameters = parameters.Skip(1).ToArray();
                }
            }

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

        protected virtual IEnumerable<(Type t, object o)> UnwrapInvocationContext(
            TInvocationContext invocationContext)
        {
            return Enumerable.Empty<(Type t, object o)>();
        }
    }
}