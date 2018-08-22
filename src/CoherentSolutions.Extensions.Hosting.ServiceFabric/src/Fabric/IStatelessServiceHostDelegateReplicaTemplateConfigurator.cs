using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostDelegateReplicaTemplateConfigurator
        : IServiceHostDelegateReplicaTemplateConfigurator
    {
        void UseEvent(
            StatelessServiceLifecycleEvent @event);

        void UseDelegateInvoker(
            Func<Delegate, IServiceProvider, IStatelessServiceHostDelegateInvoker> factoryFunc);
    }
}