namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostAsyncDelegateReplicaTemplate
        : IStatelessServiceHostAsyncDelegateReplicableTemplate,
          IServiceHostAsyncDelegateReplicaTemplate<IStatelessServiceHostAsyncDelegateReplicaTemplateConfigurator>

    {
    }
}