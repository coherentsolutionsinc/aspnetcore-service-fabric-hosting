namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostDelegateReplicator
        : ServiceHostDelegateReplicator<IStatelessServiceHostDelegateReplicableTemplate, IStatelessService, IServiceHostDelegateInvoker>,
          IStatelessServiceHostDelegateReplicator
    {
        public StatelessServiceHostDelegateReplicator(
            IStatelessServiceHostDelegateReplicableTemplate replicaTemplate)
            : base(replicaTemplate)
        {
        }
    }
}