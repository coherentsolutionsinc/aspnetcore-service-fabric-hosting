using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public interface IHybridHostBuilderConfigurator
        : IConfigurableObjectWebHostConfigurator
    {
        void UseStatefulServiceHostBuilder(
            Func<IStatefulServiceHostBuilder> factoryFunc);

        void UseStatelessServiceHostBuilder(
            Func<IStatelessServiceHostBuilder> factoryFunc);

        void UseHostSelector(
            Func<IHostSelector> factoryFunc);

        void UseHost(
            Func<IHostRunner, IHost> factoryFunc);

        void ConfigureStatefulServiceHost(
            Action<IStatefulServiceHostBuilder> configAction);

        void ConfigureStatelessServiceHost(
            Action<IStatelessServiceHostBuilder> configAction);
    }
}