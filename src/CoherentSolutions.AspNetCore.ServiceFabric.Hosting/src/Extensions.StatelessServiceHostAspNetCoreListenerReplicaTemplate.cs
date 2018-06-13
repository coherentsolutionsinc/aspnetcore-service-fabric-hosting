using System;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting
{
    public static partial class Extensions
    {
        public static IStatelessServiceHostAspNetCoreListenerReplicaTemplate Configure(
            this IStatelessServiceHostAspNetCoreListenerReplicaTemplate @this,
            Action<IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator> configAction)
        {
            @this.ConfigureObject(configAction);

            return @this;
        }
    }
}