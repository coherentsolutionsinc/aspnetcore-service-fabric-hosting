using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilder<out TServiceHist, out TConfigurator> : IConfigurableObject<TConfigurator>
        where TConfigurator : IServiceHostBuilderConfigurator
    {
        TServiceHist Build();
    }
}