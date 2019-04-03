using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextOnDataPackageRemoved
        : StatelessServiceDelegateInvocationContext<IServiceEventPayloadOnPackageRemoved<DataPackage>>,
          IStatelessServiceDelegateInvocationContextOnPackageRemoved<DataPackage>
    {
        public StatelessServiceDelegateInvocationContextOnDataPackageRemoved(
            IServiceEventPayloadOnPackageRemoved<DataPackage> payload)
            : base(StatelessServiceLifecycleEvent.OnDataPackageRemoved, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}