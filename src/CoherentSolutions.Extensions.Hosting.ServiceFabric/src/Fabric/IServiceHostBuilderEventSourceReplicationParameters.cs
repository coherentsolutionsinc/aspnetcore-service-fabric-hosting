using System;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderEventSourceParameters
    {
        Func<ServiceContext, IServiceEventSource> EventSourceFunc { get; }
    }
}