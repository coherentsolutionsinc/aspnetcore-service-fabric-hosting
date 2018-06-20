namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostDelegateReplicaTemplate
        : IStatelessServiceHostDelegateReplicableTemplate,
          IServiceHostDelegateReplicaTemplate<IStatelessServiceHostDelegateReplicaTemplateConfigurator>

    {
    }
}