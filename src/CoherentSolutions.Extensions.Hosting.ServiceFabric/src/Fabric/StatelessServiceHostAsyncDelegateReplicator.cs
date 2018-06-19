namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostAsyncDelegateReplicator
        : ServiceHostAsyncDelegateReplicator<IStatelessServiceHostAsyncDelegateReplicableTemplate, IStatelessService, IServiceHostAsyncDelegate>,
          IStatelessServiceHostAsyncDelegateReplicator
    {
        public StatelessServiceHostAsyncDelegateReplicator(
            IStatelessServiceHostAsyncDelegateReplicableTemplate replicaTemplate)
            : base(replicaTemplate)
        {
        }
    }
}