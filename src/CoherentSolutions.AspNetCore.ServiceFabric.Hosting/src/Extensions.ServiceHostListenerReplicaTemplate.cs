using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static TCaller UseEndpointName<TCaller>(
            this TCaller @this,
            string endpointName)
            where TCaller : IConfigurableObject<IServiceHostListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseEndpointName(endpointName));

            return @this;
        }
    }
}