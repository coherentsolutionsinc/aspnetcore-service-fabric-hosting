using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilder<out TServiceHost, out TConfigurator> : IConfigurableObject<TConfigurator>
        where TConfigurator : IServiceHostBuilderConfigurator
    {
        TServiceHost Build();
    }
}