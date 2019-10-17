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
    public class ServiceDelegateInvoker
        : IServiceDelegateInvoker
    {
        private readonly IEnumerable<IServiceDelegateInvocationContextRegistrant> invocationContextRegistrants;

        private readonly IServiceProvider services;

        public ServiceDelegateInvoker(
            IEnumerable<IServiceDelegateInvocationContextRegistrant> invocationContextRegistrants,
            IServiceProvider services)
        {
            this.invocationContextRegistrants = invocationContextRegistrants 
                ?? throw new ArgumentNullException(nameof(invocationContextRegistrants));

            this.services = services
                ?? throw new ArgumentNullException(nameof(services));
        }

        public Task InvokeAsync(
            Delegate @delegate,
            IServiceDelegateInvocationContext invocationContext,
            CancellationToken cancellationToken)
        {
            if (@delegate is null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            if (invocationContext is null)
            {
                throw new ArgumentNullException(nameof(invocationContext));
            }

            // This is required to support closures (for example generated using Linq.Expression) not delegates.

            var method = @delegate.GetMethodInfo();

            // Intentionally represent it as IEnumerable

            IEnumerable<ParameterInfo> parameters = method.GetParameters();

            if (method.IsStatic)
            {
                if (@delegate.Target is object)
                {
                    parameters = parameters.Skip(1);
                }
            }

            // Registering input arguments
            var injections = new Dictionary<Type, object>
            {
                [invocationContext.GetType()] = invocationContext,
                [typeof(IServiceDelegateInvocationContext)] = invocationContext,
                [typeof(CancellationToken)] = cancellationToken
            };

            foreach (var invocationContextRegistrant in this.invocationContextRegistrants)
            {
                foreach (var (t, o) in invocationContextRegistrant.GetInvocationContextRegistrations(invocationContext))
                {
                    injections[t] = o;
                }
            }

            var delegateServices = new ReplaceAwareServiceProvider(injections, this.services);

            var arguments = parameters
               .Select((pi, index) => delegateServices.GetRequiredService(pi.ParameterType))
               .ToArray();

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var result = @delegate.DynamicInvoke(arguments);
                return result is Task returnTask ? returnTask : Task.CompletedTask;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException is object)
                {
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                }

                throw;
            }
        }
    }
}