using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderRuntimeParameters<out TRuntimeRegistrant>
        where TRuntimeRegistrant : class
    {
        Func<IServiceProvider, TRuntimeRegistrant> RuntimeRegistrantFunc { get; }
    }
}