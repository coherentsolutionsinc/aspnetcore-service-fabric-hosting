namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceHostBuilder
        : IServiceHostBuilder<IStatefulServiceHost, IStatefulServiceHostBuilderConfigurator>
    {
    }
}