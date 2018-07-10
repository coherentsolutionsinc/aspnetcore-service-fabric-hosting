using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions.Support;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base
{
    public abstract class ServiceRemotingListenerTheoryItem<T>
        : TheoryItem,
          IUseRemotingImplementationTheoryExtensionSupported,
          IUseDependenciesTheoryExtensionSupported,
          IResolveDependencyTheoryExtensionSupported
        where T : IServiceHostRemotingListenerReplicaTemplateConfigurator
    {
        protected ServiceRemotingListenerTheoryItem(
            string name)
            : base(name)
        {
            this.StartExtensionsSetup();

            this.SetupExtension(new UseRemotingImplementationTheoryExtension());
            this.SetupExtension(new UseDependenciesTheoryExtension());
            this.SetupExtension(new ResolveDependencyTheoryExtension());

            this.StopExtensionsSetup();
        }

        protected virtual void ConfigureExtensions(
            T configurator)
        {
            var useRemotingImplementationExtension = this.GetExtension<IUseRemotingImplementationTheoryExtension>();
            var useDependenciesExtension = this.GetExtension<IUseDependenciesTheoryExtension>();
            var resolveDependenciesExtension = this.GetExtension<IResolveDependencyTheoryExtension>();

            configurator.UseDependencies(useDependenciesExtension.Factory);
            configurator.UseImplementation(
                provider =>
                {
                    foreach (var action in resolveDependenciesExtension.ServiceResolveDelegates)
                    {
                        action(provider);
                    }

                    return useRemotingImplementationExtension.Factory(provider);
                });
        }
    }
}