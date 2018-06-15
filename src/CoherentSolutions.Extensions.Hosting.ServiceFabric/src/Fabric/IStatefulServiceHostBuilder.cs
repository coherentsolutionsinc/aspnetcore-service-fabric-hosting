namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostBuilder
        : IServiceHostBuilder<IStatefulServiceHost, IStatefulServiceHostBuilderConfigurator>
    {
    }
}