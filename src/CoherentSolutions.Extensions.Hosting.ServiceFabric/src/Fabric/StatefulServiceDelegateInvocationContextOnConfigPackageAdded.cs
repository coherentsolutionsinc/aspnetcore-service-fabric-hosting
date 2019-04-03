using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceDelegateInvocationContextOnConfigPackageAdded
        : StatefulServiceDelegateInvocationContext<IServiceEventPayloadOnPackageAdded<ConfigurationPackage>>,
          IStatefulServiceDelegateInvocationContextOnPackageAdded<ConfigurationPackage>
    {
        public StatefulServiceDelegateInvocationContextOnConfigPackageAdded(
            IServiceEventPayloadOnPackageAdded<ConfigurationPackage> payload)
            : base(StatefulServiceLifecycleEvent.OnConfigPackageAdded, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}