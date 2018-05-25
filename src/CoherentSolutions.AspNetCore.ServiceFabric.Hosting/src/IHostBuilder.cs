using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public interface IHostBuilder : IConfigurableObject<IHostBuilderConfigurator>
    {
        IHost Build();
    }
}