using System;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public abstract class ServiceHostListenerReplicator<TReplicableTemplate, TService, TListener>
        : IServiceHostListenerReplicator<TService, TListener>
        where TReplicableTemplate : class, IServiceHostListenerReplicableTemplate<TService, TListener>
    {
        private readonly TReplicableTemplate replicableTemplate;

        protected ServiceHostListenerReplicator(
            TReplicableTemplate replicableTemplate)
        {
            this.replicableTemplate = replicableTemplate
             ?? throw new ArgumentNullException(nameof(replicableTemplate));
        }

        public TListener ReplicateFor(
            TService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return this.replicableTemplate.Activate(service);
        }
    }
}