using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostAsyncDelegateReplicator<TReplicableTemplate, TService, TDelegate>
        : IServiceHostAsyncDelegateReplicator<TService, TDelegate>
        where TReplicableTemplate : class, IServiceHostAsyncDelegateReplicableTemplate<TService, TDelegate>
    {
        private readonly TReplicableTemplate replicableTemplate;

        protected ServiceHostAsyncDelegateReplicator(
            TReplicableTemplate replicableTemplate)
        {
            this.replicableTemplate = replicableTemplate
             ?? throw new ArgumentNullException(nameof(replicableTemplate));
        }

        public TDelegate ReplicateFor(
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