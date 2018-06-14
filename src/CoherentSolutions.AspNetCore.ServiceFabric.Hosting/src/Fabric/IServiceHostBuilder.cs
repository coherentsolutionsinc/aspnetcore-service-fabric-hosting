using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilder<out TServiceHost, out TConfigurator> : IConfigurableObject<TConfigurator>
        where TConfigurator : IServiceHostBuilderConfigurator
    {
        TServiceHost Build();
    }
}