using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public sealed class StatefulServiceDelegateTheoryItem
        : ServiceDelegateTheoryItem<IStatefulServiceHostDelegateReplicaTemplateConfigurator>
    {
        public StatefulServiceDelegateTheoryItem()
            : base("StatefulService-Delegate")
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
                                    c.UseRuntimeRegistrant(Tools.StatefulRuntimeRegistrant);
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