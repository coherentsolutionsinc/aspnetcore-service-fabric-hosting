using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static TCaller UseServiceName<TCaller>(
            this TCaller @this,
            string serviceName)
            where TCaller : IConfigurableObject<IServiceHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseServiceName(serviceName));

            return @this;
        }
    }
}