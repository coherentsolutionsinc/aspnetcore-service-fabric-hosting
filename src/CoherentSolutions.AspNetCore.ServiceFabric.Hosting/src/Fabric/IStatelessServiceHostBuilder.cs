namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatelessServiceHostBuilder
        : IServiceHostBuilder<IStatelessServiceHost, IStatelessServiceHostBuilderConfigurator>
    {
    }
}