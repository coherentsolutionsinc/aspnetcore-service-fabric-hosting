using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public interface IHybridHostBuilder : IConfigurableObject<IHybridHostBuilderConfigurator>
    {
        IHost Build();
    }
}