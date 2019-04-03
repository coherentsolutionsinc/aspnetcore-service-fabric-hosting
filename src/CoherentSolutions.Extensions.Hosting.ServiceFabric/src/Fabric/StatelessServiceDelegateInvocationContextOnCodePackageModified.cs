using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextOnCodePackageModified
        : StatelessServiceDelegateInvocationContext<IServiceEventPayloadOnPackageModified<CodePackage>>,
          IStatelessServiceDelegateInvocationContextOnPackageModified<CodePackage>
    {
        public StatelessServiceDelegateInvocationContextOnCodePackageModified(
            IServiceEventPayloadOnPackageModified<CodePackage> payload)
            : base(StatelessServiceLifecycleEvent.OnCodePackageModified, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}