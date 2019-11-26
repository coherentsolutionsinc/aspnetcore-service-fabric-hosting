using System;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderRuntimeConfigurator<TRuntimeRegistrant>
        where TRuntimeRegistrant : class
    {
        void UseRuntimeRegistrant(
            Func<TRuntimeRegistrant> factoryFunc);
    }
}