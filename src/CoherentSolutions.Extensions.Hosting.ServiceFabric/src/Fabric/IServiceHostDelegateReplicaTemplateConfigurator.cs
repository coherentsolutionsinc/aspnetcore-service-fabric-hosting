using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostDelegateReplicaTemplateConfigurator
        : IServiceHostBuilderBlockConfigurator
    {
        void UseDelegate(
            Delegate @delegate);

        void UseDelegateInvoker(
            Func<IServiceProvider, IServiceDelegateInvoker> factoryFunc);
    }
}