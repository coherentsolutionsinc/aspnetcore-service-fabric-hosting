namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostAsyncDelegateReplicaTemplate
        : IStatefulServiceHostAsyncDelegateReplicableTemplate,
          IServiceHostAsyncDelegateReplicaTemplate<IStatefulServiceHostAsyncDelegateReplicaTemplateConfigurator>

    {
    }
}