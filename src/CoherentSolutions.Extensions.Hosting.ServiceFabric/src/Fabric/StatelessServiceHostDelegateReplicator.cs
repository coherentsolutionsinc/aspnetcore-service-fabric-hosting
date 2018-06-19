namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostDelegateReplicator
        : ServiceHostDelegateReplicator<IStatelessServiceHostDelegateReplicableTemplate, IStatelessService, IServiceHostDelegate>,
          IStatelessServiceHostDelegateReplicator
    {
        public StatelessServiceHostDelegateReplicator(
            IStatelessServiceHostDelegateReplicableTemplate replicaTemplate)
            : base(replicaTemplate)
        {
        }
    }
}