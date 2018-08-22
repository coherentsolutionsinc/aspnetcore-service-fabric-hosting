using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostDelegateInvoker
        : ServiceHostDelegateInvoker,
          IStatefulServiceHostDelegateInvoker
    {
        private readonly IServiceProvider services;

        public StatefulServiceHostDelegateInvoker(
            Delegate @delegate,
            IServiceProvider service)
            : base(@delegate)
        {
            this.services = service
             ?? throw new ArgumentNullException(nameof(service));
        }

        public Task InvokeAsync(
            IStatefulServiceDelegateInvocationContext context,
            CancellationToken cancellationToken)
        {
            var invocationArgumentsProvider = new ReplaceAwareServiceProvider(
                new Dictionary<Type, object>
                {
                    [typeof(IStatefulServiceDelegateInvocationContext)] = context
                },
                this.services);

            var invocation = this.CreateInvocation(invocationArgumentsProvider);

            return invocation(cancellationToken);
        }
    }
}