using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostEventSourceReplicator<TReplicableTemplate, TServiceInformation, TEventSource>
        : IServiceHostEventSourceReplicator<TServiceInformation, TEventSource>
        where TServiceInformation : IServiceInformation
        where TReplicableTemplate : class, IServiceHostEventSourceReplicableTemplate<TServiceInformation, TEventSource>
    {
        private readonly TReplicableTemplate replicableTemplate;

        protected ServiceHostEventSourceReplicator(
            TReplicableTemplate replicableTemplate)
        {
            this.replicableTemplate = replicableTemplate
             ?? throw new ArgumentNullException(nameof(replicableTemplate));
        }

        public TEventSource ReplicateFor(
            TServiceInformation serviceInformation)
        {
            if (serviceInformation == null)
            {
                throw new ArgumentNullException(nameof(serviceInformation));
            }

            return this.replicableTemplate.Activate(serviceInformation);
        }
    }
}