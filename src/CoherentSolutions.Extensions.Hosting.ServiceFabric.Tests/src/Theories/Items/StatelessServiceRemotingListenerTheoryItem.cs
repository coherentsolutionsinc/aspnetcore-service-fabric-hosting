using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public sealed class StatelessServiceRemotingListenerTheoryItem
        : ServiceRemotingListenerTheoryItem<IStatelessServiceHostRemotingListenerReplicaTemplateConfigurator>
    {
        public StatelessServiceRemotingListenerTheoryItem()
            : base("StatelessService-RemotingListener")
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
                                    c.DefineRemotingListener(
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