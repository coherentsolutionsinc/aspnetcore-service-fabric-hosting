using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnDataPackageAdded
        : StatefulServiceDelegateInvocationContext<IServiceEventPayloadOnPackageAdded<DataPackage>>,
          IStatefulServiceDelegateInvocationContextOnPackageAdded<DataPackage>
    {
        public StatefulServiceDelegateInvocationContextOnDataPackageAdded(
            IServiceEventPayloadOnPackageAdded<DataPackage> payload)
            : base(StatefulServiceLifecycleEvent.OnDataPackageAdded, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}