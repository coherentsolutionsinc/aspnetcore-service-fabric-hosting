using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnCodePackageRemoved
        : StatefulServiceDelegateInvocationContext<IServiceEventPayloadOnPackageRemoved<CodePackage>>,
          IStatefulServiceDelegateInvocationContextOnPackageRemoved<CodePackage>
    {
        public StatefulServiceDelegateInvocationContextOnCodePackageRemoved(
            IServiceEventPayloadOnPackageRemoved<CodePackage> payload)
            : base(StatefulServiceLifecycleEvent.OnCodePackageRemoved, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}