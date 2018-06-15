namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatelessServiceHostBuilder
        : IServiceHostBuilder<IStatelessServiceHost, IStatelessServiceHostBuilderConfigurator>
    {
    }
}