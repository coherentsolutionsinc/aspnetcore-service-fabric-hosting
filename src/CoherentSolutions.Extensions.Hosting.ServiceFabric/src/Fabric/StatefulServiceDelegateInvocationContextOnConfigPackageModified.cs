using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnConfigPackageModified
        : StatefulServiceDelegateInvocationContext<IServiceEventPayloadOnPackageModified<ConfigurationPackage>>,
          IStatefulServiceDelegateInvocationContextOnPackageModified<ConfigurationPackage>
    {
        public StatefulServiceDelegateInvocationContextOnConfigPackageModified(
            IServiceEventPayloadOnPackageModified<ConfigurationPackage> payload)
            : base(StatefulServiceLifecycleEvent.OnConfigPackageModified, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}