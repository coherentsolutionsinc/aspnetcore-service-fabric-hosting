namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostDelegateReplicaTemplate
        : IStatefulServiceHostDelegateReplicableTemplate,
          IServiceHostDelegateReplicaTemplate<IStatefulServiceHostDelegateReplicaTemplateConfigurator>

    {
    }
}