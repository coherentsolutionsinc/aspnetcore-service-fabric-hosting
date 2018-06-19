using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceHostBuilderConfigurator
        : IConfigurableObjectDependenciesConfigurator
    {
        void UseServiceType(
            string serviceTypeName);
    }
}