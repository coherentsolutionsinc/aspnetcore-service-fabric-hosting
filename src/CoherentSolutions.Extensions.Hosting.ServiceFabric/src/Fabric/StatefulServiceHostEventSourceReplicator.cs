namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostEventSourceReplicator
        : ServiceHostEventSourceReplicator<IStatefulServiceHostEventSourceReplicableTemplate, IStatefulServiceInformation, StatefulServiceEventSource>,
          IStatefulServiceHostEventSourceReplicator
    {
        public StatefulServiceHostEventSourceReplicator(
            IStatefulServiceHostEventSourceReplicableTemplate replicableTemplate)
            : base(replicableTemplate)
        {
        }
    }
}