using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextOnDataPackageAdded
        : StatelessServiceDelegateInvocationContext<IServiceEventPayloadOnPackageAdded<DataPackage>>,
          IStatelessServiceDelegateInvocationContextOnPackageAdded<DataPackage>
    {
        public StatelessServiceDelegateInvocationContextOnDataPackageAdded(
            IServiceEventPayloadOnPackageAdded<DataPackage> payload)
            : base(StatelessServiceLifecycleEvent.OnDataPackageAdded, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}