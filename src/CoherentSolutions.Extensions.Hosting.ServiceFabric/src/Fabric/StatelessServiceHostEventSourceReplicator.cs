namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostEventSourceReplicator
        : ServiceHostEventSourceReplicator<IStatelessServiceHostEventSourceReplicableTemplate, IStatelessServiceInformation, StatelessServiceEventSource>,
          IStatelessServiceHostEventSourceReplicator
    {
        public StatelessServiceHostEventSourceReplicator(
            IStatelessServiceHostEventSourceReplicableTemplate replicableTemplate)
            : base(replicableTemplate)
        {
        }
    }
}