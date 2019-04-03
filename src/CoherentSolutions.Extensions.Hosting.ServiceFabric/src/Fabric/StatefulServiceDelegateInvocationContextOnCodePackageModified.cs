using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnCodePackageModified
        : StatefulServiceDelegateInvocationContext<IServiceEventPayloadOnPackageModified<CodePackage>>,
          IStatefulServiceDelegateInvocationContextOnPackageModified<CodePackage>
    {
        public StatefulServiceDelegateInvocationContextOnCodePackageModified(
            IServiceEventPayloadOnPackageModified<CodePackage> payload)
            : base(StatefulServiceLifecycleEvent.OnCodePackageModified, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}