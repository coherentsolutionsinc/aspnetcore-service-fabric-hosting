using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static IStatefulServiceHostAspNetCoreListenerReplicaTemplate Configure(
            this IStatefulServiceHostAspNetCoreListenerReplicaTemplate @this,
            Action<IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }
    }
}