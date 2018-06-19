namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostAsyncDelegateReplicator
        : ServiceHostAsyncDelegateReplicator<IStatefulServiceHostAsyncDelegateReplicableTemplate, IStatefulService, IServiceHostAsyncDelegate>,
          IStatefulServiceHostAsyncDelegateReplicator
    {
        public StatefulServiceHostAsyncDelegateReplicator(
            IStatefulServiceHostAsyncDelegateReplicableTemplate replicaTemplate)
            : base(replicaTemplate)
        {
        }
    }
}