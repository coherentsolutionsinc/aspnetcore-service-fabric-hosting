namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostEventSourceReplicaTemplate
        : IStatefulServiceHostEventSourceReplicableTemplate,
          IServiceHostEventSourceReplicaTemplate<IStatefulServiceHostEventSourceReplicaTemplateConfigurator>
    {
        
    }
}