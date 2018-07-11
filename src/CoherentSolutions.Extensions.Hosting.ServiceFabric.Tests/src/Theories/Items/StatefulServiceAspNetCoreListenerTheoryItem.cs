﻿using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public sealed class StatefulServiceAspNetCoreListenerTheoryItem
        : ServiceAspNetCoreListenerTheoryItem<IStatefulServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
    {
        public StatefulServiceAspNetCoreListenerTheoryItem()
            : base("StatefulService-AspNetCoreListener")
        {
            this.SetupConfig(
                builder =>
                {
                    builder.DefineStatefulService(
                        serviceBuilder =>
                        {
                            serviceBuilder.ConfigureObject(
                                c =>
                                {
                                    c.UseRuntimeRegistrant(Tools.GetStatefulRuntimeRegistrantFunc());
                                    c.DefineAspNetCoreListener(
                                        listenerBuilder =>
                                        {
                                            listenerBuilder.ConfigureObject(this.ConfigureExtensions);
                                        });
                                });
                        });
                });
        }
    }
}