using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostDelegateReplicaTemplateParameters
        : IServiceHostDelegateReplicaTemplateParameters
    {
        StatelessServiceLifecycleEvent Event { get; }

        Func<Delegate, IServiceProvider, IStatelessServiceHostDelegateInvoker> DelegateInvokerFunc { get; }
    }
}