namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostDelegateReplicator
        : ServiceHostDelegateReplicator<IStatefulServiceHostDelegateReplicableTemplate, IStatefulService, StatefulServiceDelegate>,
          IStatefulServiceHostDelegateReplicator
    {
        public StatefulServiceHostDelegateReplicator(
            IStatefulServiceHostDelegateReplicableTemplate replicaTemplate)
            : base(replicaTemplate)
        {
        }
    }
}