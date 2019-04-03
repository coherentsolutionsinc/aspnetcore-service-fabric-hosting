using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnDataPackageModified
        : StatefulServiceDelegateInvocationContext<IServiceEventPayloadOnPackageModified<DataPackage>>,
          IStatefulServiceDelegateInvocationContextOnPackageModified<DataPackage>
    {
        public StatefulServiceDelegateInvocationContextOnDataPackageModified(
            IServiceEventPayloadOnPackageModified<DataPackage> payload)
            : base(StatefulServiceLifecycleEvent.OnDataPackageModified, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}