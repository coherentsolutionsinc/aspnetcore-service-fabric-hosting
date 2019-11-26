using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderRuntimeParameters<out TRuntimeRegistrant>
        where TRuntimeRegistrant : class
    {
        Func<TRuntimeRegistrant> RuntimeRegistrantFunc { get; }
    }
}