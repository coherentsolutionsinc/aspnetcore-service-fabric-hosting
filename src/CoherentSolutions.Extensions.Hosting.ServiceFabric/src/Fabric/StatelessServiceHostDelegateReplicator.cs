namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostDelegateReplicator
        : ServiceHostDelegateReplicator<IStatelessServiceHostDelegateReplicableTemplate, IStatelessService, StatelessServiceDelegate>,
          IStatelessServiceHostDelegateReplicator
    {
        public StatelessServiceHostDelegateReplicator(
            IStatelessServiceHostDelegateReplicableTemplate replicaTemplate)
            : base(replicaTemplate)
        {
        }
    }
}