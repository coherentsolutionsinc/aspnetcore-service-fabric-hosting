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
                                    c.UseRuntimeRegistrant(Tools.StatelessRuntimeRegistrant);
                                    c.DefineAspNetCoreListener(
                                        listenerBuilder =>
                                        {
                                            listenerBuilder.UseCommunicationListener(Tools.AspNetCoreCommunicationListenerFunc);
                                            listenerBuilder.ConfigureObject(this.ConfigureExtensions);
                                        });
                                });
                        });
                });
        }
    }
}