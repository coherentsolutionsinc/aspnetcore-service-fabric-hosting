using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnDataPackageRemoved
        : StatefulServiceDelegateInvocationContext<IServiceEventPayloadOnPackageRemoved<DataPackage>>,
          IStatefulServiceDelegateInvocationContextOnPackageRemoved<DataPackage>
    {
        public StatefulServiceDelegateInvocationContextOnDataPackageRemoved(
            IServiceEventPayloadOnPackageRemoved<DataPackage> payload)
            : base(StatefulServiceLifecycleEvent.OnDataPackageRemoved, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}