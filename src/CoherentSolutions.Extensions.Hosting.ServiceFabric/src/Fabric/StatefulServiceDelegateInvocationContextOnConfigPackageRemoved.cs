using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnConfigPackageRemoved
        : StatefulServiceDelegateInvocationContext<IServiceEventPayloadOnPackageRemoved<ConfigurationPackage>>,
          IStatefulServiceDelegateInvocationContextOnPackageRemoved<ConfigurationPackage>
    {
        public StatefulServiceDelegateInvocationContextOnConfigPackageRemoved(
            IServiceEventPayloadOnPackageRemoved<ConfigurationPackage> payload)
            : base(StatefulServiceLifecycleEvent.OnConfigPackageRemoved, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}