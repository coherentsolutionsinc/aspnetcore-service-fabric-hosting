using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextOnConfigPackageModified
        : StatelessServiceDelegateInvocationContext<IServiceEventPayloadOnPackageModified<ConfigurationPackage>>,
          IStatelessServiceDelegateInvocationContextOnPackageModified<ConfigurationPackage>
    {
        public StatelessServiceDelegateInvocationContextOnConfigPackageModified(
            IServiceEventPayloadOnPackageModified<ConfigurationPackage> payload)
            : base(StatelessServiceLifecycleEvent.OnConfigPackageModified, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}