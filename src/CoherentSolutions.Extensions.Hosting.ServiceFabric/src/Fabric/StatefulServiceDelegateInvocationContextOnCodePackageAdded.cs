using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnCodePackageAdded
        : StatefulServiceDelegateInvocationContext<IServiceEventPayloadOnPackageAdded<CodePackage>>,
          IStatefulServiceDelegateInvocationContextOnPackageAdded<CodePackage>
    {
        public StatefulServiceDelegateInvocationContextOnCodePackageAdded(
            IServiceEventPayloadOnPackageAdded<CodePackage> payload)
            : base(StatefulServiceLifecycleEvent.OnCodePackageAdded, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}