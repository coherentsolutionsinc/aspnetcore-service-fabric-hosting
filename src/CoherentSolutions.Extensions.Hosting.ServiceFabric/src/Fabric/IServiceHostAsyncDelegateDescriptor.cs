using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostAsyncDelegateDescriptor
    {
        Action<IServiceHostAsyncDelegateReplicaTemplate<IServiceHostAsyncDelegateReplicaTemplateConfigurator>> ConfigAction { get; }
    }
}