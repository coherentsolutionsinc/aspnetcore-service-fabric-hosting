using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostEventSourceReplicaTemplateConfigurator
        : IServiceHostReplicaTemplateConfigurator
    {
        void UseImplementation(
            Func<IServiceProvider, IServiceEventSource> factoryFunc);
    }
}