using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextOnCodePackageRemoved
        : StatelessServiceDelegateInvocationContext<IServiceEventPayloadOnPackageRemoved<CodePackage>>,
          IStatelessServiceDelegateInvocationContextOnPackageRemoved<CodePackage>
    {
        public StatelessServiceDelegateInvocationContextOnCodePackageRemoved(
            IServiceEventPayloadOnPackageRemoved<CodePackage> payload)
            : base(StatelessServiceLifecycleEvent.OnCodePackageRemoved, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}