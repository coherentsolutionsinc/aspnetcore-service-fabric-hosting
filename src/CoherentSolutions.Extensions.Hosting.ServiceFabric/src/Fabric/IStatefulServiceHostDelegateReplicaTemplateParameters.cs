using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostDelegateReplicaTemplateParameters
        : IServiceHostDelegateReplicaTemplateParameters
    {
        StatefulServiceLifecycleEvent Event { get; }

        Func<Delegate, IServiceProvider, IStatefulServiceHostDelegateInvoker> DelegateInvokerFunc { get; }
    }
}