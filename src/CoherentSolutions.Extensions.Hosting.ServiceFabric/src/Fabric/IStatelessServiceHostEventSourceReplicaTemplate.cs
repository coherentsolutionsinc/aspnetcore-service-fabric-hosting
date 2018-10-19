namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostEventSourceReplicaTemplate
        : IStatelessServiceHostEventSourceReplicableTemplate,
          IServiceHostEventSourceReplicaTemplate<IStatelessServiceHostEventSourceReplicaTemplateConfigurator>
    {
    }
}