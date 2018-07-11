using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public sealed class StatelessServiceAspNetCoreListenerTheoryItem
        : ServiceAspNetCoreListenerTheoryItem<IStatelessServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
    {
        public StatelessServiceAspNetCoreListenerTheoryItem()
            : base("StatelessService-AspNetCoreListener")
        {
            this.SetupConfig(
                builder =>
                {
                    builder.DefineStatelessService(
                        serviceBuilder =>
                        {
                            serviceBuilder.ConfigureObject(
                                c =>
                                {
                                    c.UseRuntimeRegistrant(Tools.GetStatelessRuntimeRegistrantFunc());
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