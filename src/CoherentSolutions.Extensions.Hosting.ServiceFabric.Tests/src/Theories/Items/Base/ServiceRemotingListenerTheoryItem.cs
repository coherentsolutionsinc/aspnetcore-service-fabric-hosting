using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions.Support;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

using Moq;

using IRemotingService = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items.Base
{
    public abstract class ServiceRemotingListenerTheoryItem<T>
        : ServiceListenerTheoryItem<T>,
          IUseRemotingCommunicationListenerTheoryExtensionSupported,
          IUseRemotingImplementationTheoryExtensionSupported,
          IUseRemotingSerializerTheoryExtensionSupported,
          IUseRemotingSettingsTheoryExtensionSupported,
          IUseDependenciesTheoryExtensionSupported,
          IPickDependencyTheoryExtensionSupported,
          IPickRemotingImplementationTheoryExtensionSupported,
          IPickRemotingSerializerTheoryExtensionSupported,
          IPickRemotingSettingsTheoryExtensionSupported
        where T : IServiceHostRemotingListenerReplicaTemplateConfigurator
    {
        protected ServiceRemotingListenerTheoryItem(
            string name)
            : base(name)
        {
        }

        protected override void InitializeExtensions()
        {
            base.InitializeExtensions();

            this.SetupExtension(new UseRemotingCommunicationListenerTheoryExtension());
            this.SetupExtension(new UseRemotingImplementationTheoryExtension());
            this.SetupExtension(new UseRemotingSerializerTheoryExtension());
            this.SetupExtension(new UseRemotingSettingsTheoryExtension());
            this.SetupExtension(new UseDependenciesTheoryExtension());
            this.SetupExtension(new PickDependencyTheoryExtension());
            this.SetupExtension(new PickListenerEndpointTheoryExtension());
            this.SetupExtension(new PickRemotingImplementationTheoryExtension());
            this.SetupExtension(new PickRemotingSerializerTheoryExtension());
            this.SetupExtension(new PickRemotingSettingsTheoryExtension());
        }

        protected override void ConfigureExtensions(
            T configurator)
        {
            base.ConfigureExtensions(configurator);

            var useRemotingCommunicationListenerExtension = this.GetExtension<IUseRemotingCommunicationListenerTheoryExtension>();
            var useRemotingImplementationExtension = this.GetExtension<IUseRemotingImplementationTheoryExtension>();
            var useRemotingSerializerExtension = this.GetExtension<IUseRemotingSerializerTheoryExtension>();
            var useRemotingSettingsExtension = this.GetExtension<IUseRemotingSettingsTheoryExtension>();
            var useDependenciesExtension = this.GetExtension<IUseDependenciesTheoryExtension>();
            var pickDependenciesExtension = this.GetExtension<IPickDependencyTheoryExtension>();
            var pickListenerEndpointExtension = this.GetExtension<IPickListenerEndpointTheoryExtension>();
            var pickRemotingImplementation = this.GetExtension<IPickRemotingImplementationTheoryExtension>();
            var pickRemotingSerializerExtension = this.GetExtension<IPickRemotingSerializerTheoryExtension>();
            var pickRemotingSettingsExtension = this.GetExtension<IPickRemotingSettingsTheoryExtension>();

            configurator.UseDependencies(useDependenciesExtension.Factory);
            configurator.UseHandler(
                provider =>
                {
                    pickRemotingImplementation.PickAction(provider.GetService<IRemotingService>());

                    return new Mock<IServiceRemotingMessageHandler>().Object;
                });
            configurator.UseCommunicationListener(
                (
                    context,
                    build) =>
                {
                    var options = build(context);

                    pickListenerEndpointExtension.PickAction(options.ListenerSettings.EndpointResourceName);
                    pickRemotingSerializerExtension.PickAction(options.MessageSerializationProvider);
                    pickRemotingSettingsExtension.PickAction(options.ListenerSettings);

                    return useRemotingCommunicationListenerExtension.Factory(context, build);
                });
            configurator.UseImplementation(
                provider =>
                {
                    foreach (var action in pickDependenciesExtension.PickActions)
                    {
                        action(provider);
                    }

                    return useRemotingImplementationExtension.Factory(provider);
                });
            configurator.UseSerializer(useRemotingSerializerExtension.Factory);
            configurator.UseSettings(useRemotingSettingsExtension.Factory);
        }
    }
}