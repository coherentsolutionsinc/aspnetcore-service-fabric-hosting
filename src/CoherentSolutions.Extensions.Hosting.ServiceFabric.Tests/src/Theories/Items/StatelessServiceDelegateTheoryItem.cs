using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public sealed class StatelessServiceDelegateTheoryItem
        : ServiceDelegateTheoryItem<IStatelessServiceHostDelegateReplicaTemplateConfigurator>
    {
        public StatelessServiceDelegateTheoryItem()
            : base("StatelessService-Delegate")
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
                                    c.DefineDelegate(
                                        delegateBuilder =>
                                        {
                                            delegateBuilder
                                               .ConfigureObject(
                                                    this.ConfigureExtensions);
                                        });
                                });
                        });
                });
        }
    }
}