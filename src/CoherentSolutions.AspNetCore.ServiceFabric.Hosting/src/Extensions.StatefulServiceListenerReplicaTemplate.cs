using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static TCaller UseListenerOnSecondary<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IStatefulServiceListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseListenerOnSecondary());

            return @this;
        }
    }
}