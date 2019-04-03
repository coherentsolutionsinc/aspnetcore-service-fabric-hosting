using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextOnConfigPackageAdded
        : StatelessServiceDelegateInvocationContext<IServiceEventPayloadOnPackageAdded<ConfigurationPackage>>,
          IStatelessServiceDelegateInvocationContextOnPackageAdded<ConfigurationPackage>
    {
        public StatelessServiceDelegateInvocationContextOnConfigPackageAdded(
            IServiceEventPayloadOnPackageAdded<ConfigurationPackage> payload)
            : base(StatelessServiceLifecycleEvent.OnConfigPackageAdded, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}