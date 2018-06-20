using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateDescriptor
    {
        Action<IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>> ConfigAction { get; }
    }
}