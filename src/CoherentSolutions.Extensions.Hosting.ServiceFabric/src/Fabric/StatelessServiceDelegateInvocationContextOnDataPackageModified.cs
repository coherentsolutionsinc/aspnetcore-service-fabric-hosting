using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextOnDataPackageModified
        : StatelessServiceDelegateInvocationContext<IServiceEventPayloadOnPackageModified<DataPackage>>,
          IStatelessServiceDelegateInvocationContextOnPackageModified<DataPackage>
    {
        public StatelessServiceDelegateInvocationContextOnDataPackageModified(
            IServiceEventPayloadOnPackageModified<DataPackage> payload)
            : base(StatelessServiceLifecycleEvent.OnDataPackageModified, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}