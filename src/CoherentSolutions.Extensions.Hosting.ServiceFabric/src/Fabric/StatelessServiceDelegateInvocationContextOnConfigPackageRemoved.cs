using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceDelegateInvocationContextOnConfigPackageRemoved
        : StatelessServiceDelegateInvocationContext<IServiceEventPayloadOnPackageRemoved<ConfigurationPackage>>,
          IStatelessServiceDelegateInvocationContextOnPackageRemoved<ConfigurationPackage>
    {
        public StatelessServiceDelegateInvocationContextOnConfigPackageRemoved(
            IServiceEventPayloadOnPackageRemoved<ConfigurationPackage> payload)
            : base(StatelessServiceLifecycleEvent.OnConfigPackageRemoved, payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }
        }
    }
}