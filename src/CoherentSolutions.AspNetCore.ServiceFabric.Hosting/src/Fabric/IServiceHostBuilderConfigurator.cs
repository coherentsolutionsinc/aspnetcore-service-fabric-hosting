using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IServiceHostBuilderConfigurator
        : IConfigurableObjectWebHostConfigurator
    {
        void UseServiceName(
            string serviceName);
    }
}