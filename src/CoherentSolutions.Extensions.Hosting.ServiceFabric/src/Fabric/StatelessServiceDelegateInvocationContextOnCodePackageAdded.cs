using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextOnCodePackageAdded
        : StatelessServiceDelegateInvocationContext<IServiceEventPayloadOnPackageAdded<CodePackage>>,
          IStatelessServiceDelegateInvocationContextOnPackageAdded<CodePackage>
    {
        public StatelessServiceDelegateInvocationContextOnCodePackageAdded(
            IServiceEventPayloadOnPackageAdded<CodePackage> payload)
            : base(StatelessServiceLifecycleEvent.OnCodePackageAdded, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}