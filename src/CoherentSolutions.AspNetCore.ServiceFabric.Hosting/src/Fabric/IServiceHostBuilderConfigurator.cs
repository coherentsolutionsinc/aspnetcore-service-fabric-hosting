using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderConfigurator
        : IConfigurableObjectDependenciesConfigurator,
          IConfigurableObjectWebHostConfigurator
    {
        void UseServiceName(
            string serviceName);
    }
}