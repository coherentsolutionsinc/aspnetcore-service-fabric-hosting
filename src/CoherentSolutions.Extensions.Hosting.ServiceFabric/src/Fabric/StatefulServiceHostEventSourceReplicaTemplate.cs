namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostEventSourceReplicaTemplate<TParameters, TConfigurator> 
        : ServiceHostEventSourceReplicaTemplate<TParameters, TConfigurator>
        where TParameters : IServiceHostEventSourceReplicaTemplateParameters
        where TConfigurator : IServiceHostEventSourceReplicaTemplateConfigurator
    {
    }
}