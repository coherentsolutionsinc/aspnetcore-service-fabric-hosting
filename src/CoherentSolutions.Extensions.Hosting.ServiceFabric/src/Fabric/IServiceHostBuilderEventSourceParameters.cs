using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderEventSourceParameters<TReplicaTemplate>
        where TReplicaTemplate : IServiceHostEventSourceReplicaTemplate<IServiceHostEventSourceReplicaTemplateConfigurator>
    {
        Func<TReplicaTemplate> EventSourceReplicaTemplateFunc
        {
            get;
        }

        Action<TReplicaTemplate> EventSourceSetupAction
        {
            get;
        }
    }
}