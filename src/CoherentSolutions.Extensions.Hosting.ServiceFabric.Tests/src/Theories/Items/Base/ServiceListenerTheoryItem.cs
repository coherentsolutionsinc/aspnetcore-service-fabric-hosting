using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions.Support;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base
{
    public abstract class ServiceListenerTheoryItem<T>
        : TheoryItem,
          IPickListenerEndpointTheoryExtensionSupported
        where T : IServiceHostListenerReplicaTemplateConfigurator
    {
        protected ServiceListenerTheoryItem(
            string name)
            : base(name)
        {
        }

        protected override void InitializeExtensions()
        {
            this.SetupExtension(new UseListenerEndpointTheoryExtension());
        }

        protected virtual void ConfigureExtensions(
            T configurator)
        {
            var useListenerEndpointExtension = this.GetExtension<IUseListenerEndpointTheoryExtension>();

            configurator.UseEndpoint(useListenerEndpointExtension.Endpoint);
        }
    }
}