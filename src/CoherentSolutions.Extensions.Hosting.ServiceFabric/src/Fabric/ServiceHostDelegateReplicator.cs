using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostDelegateReplicator<TReplicableTemplate, TService, TDelegate>
        : IServiceHostDelegateReplicator<TService, TDelegate>
        where TReplicableTemplate : class, IServiceHostDelegateReplicableTemplate<TService, TDelegate>
    {
        private readonly TReplicableTemplate replicableTemplate;

        protected ServiceHostDelegateReplicator(
            TReplicableTemplate replicableTemplate)
        {
            this.replicableTemplate = replicableTemplate
             ?? throw new ArgumentNullException(nameof(replicableTemplate));
        }

        public TDelegate ReplicateFor(
            TService service)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return this.replicableTemplate.Activate(service);
        }
    }
}