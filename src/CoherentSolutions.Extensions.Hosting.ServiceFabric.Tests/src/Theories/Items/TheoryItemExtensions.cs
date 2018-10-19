using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Mocks;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public static class TheoryItemExtensions
    {
        public static TheoryItem SetupConfigAsStatefulService(
            this TheoryItem @this,
            Action<IStatefulServiceHostBuilderConfigurator, TheoryItem.TheoryItemExtensionProvider> configAction)
        {
            return @this
               .SetupConfig(
                    (
                        builder,
                        extensions) =>
                    {
                        builder.DefineStatefulService(
                            serviceBuilder =>
                            {
                                serviceBuilder
                                   .ConfigureObject(
                                        c =>
                                        {
                                            c.UseRuntimeRegistrant(() => new MockStatefulServiceRuntimeRegistrant());

                                            configAction(c, extensions);
                                        });
                            });
                    });
        }

        public static TheoryItem SetupConfigAsStatelessService(
            this TheoryItem @this,
            Action<IStatelessServiceHostBuilderConfigurator, TheoryItem.TheoryItemExtensionProvider> configAction)
        {
            return @this
               .SetupConfig(
                    (
                        builder,
                        provider) =>
                    {
                        builder.DefineStatelessService(
                            serviceBuilder =>
                            {
                                serviceBuilder
                                   .ConfigureObject(
                                        c =>
                                        {
                                            c.UseRuntimeRegistrant(() => new MockStatelessServiceRuntimeRegistrant());

                                            configAction(c, provider);
                                        });
                            });
                    });
        }

        public static TheoryItem SetupExtensionsAsStatefulDelegate(
            this TheoryItem @this)
        {
            return @this
               .SetupExtension(new UseStatefulDelegateInvokerTheoryExtension())
               .SetupExtension(new UseStatefulDelegateEventTheoryExtension())
               .SetupExtensionsAsDelegate();
        }

        public static TheoryItem SetupExtensionsAsStatelessDelegate(
            this TheoryItem @this)
        {
            return @this
               .SetupExtension(new UseStatelessDelegateInvokerTheoryExtension())
               .SetupExtension(new UseStatelessDelegateEventTheoryExtension())
               .SetupExtensionsAsDelegate();
        }

        public static TheoryItem SetupExtensionsAsAspNetCoreListener(
            this TheoryItem @this)
        {
            return @this
               .SetupExtension(new UseListenerEndpointTheoryExtension())
               .SetupExtension(new UseAspNetCoreListenerCommunicationListenerTheoryExtension())
               .SetupExtension(new UseAspNetCoreListenerWebHostBuilderTheoryExtension())
               .SetupExtension(new ConfigureDependenciesTheoryExtension())
               .SetupExtension(new PickDependencyTheoryExtension())
               .SetupExtension(new PickListenerEndpointTheoryExtension());
        }

        public static TheoryItem SetupExtensionsAsRemotingListener(
            this TheoryItem @this)
        {
            return @this
               .SetupExtension(new UseListenerEndpointTheoryExtension())
               .SetupExtension(new UseRemotingListenerCommunicationListenerTheoryExtension())
               .SetupExtension(new UseRemotingListenerImplementationTheoryExtension())
               .SetupExtension(new UseRemotingListenerSettingsTheoryExtension())
               .SetupExtension(new UseRemotingListenerSerializationProviderTheoryExtension())
               .SetupExtension(new UseRemotingListenerHandlerTheoryExtension())
               .SetupExtension(new UseDependenciesTheoryExtension())
               .SetupExtension(new ConfigureDependenciesTheoryExtension())
               .SetupExtension(new PickDependencyTheoryExtension())
               .SetupExtension(new PickListenerEndpointTheoryExtension())
               .SetupExtension(new PickRemotingListenerImplementationTheoryExtension())
               .SetupExtension(new PickRemotingListenerSettingsTheoryExtension())
               .SetupExtension(new PickRemotingListenerSerializationProviderTheoryExtension())
               .SetupExtension(new PickRemotingListenerHandlerTheoryExtension());
        }

        public static TheoryItem SetupExtensionsAsEventSource(
            this TheoryItem @this)
        {
            return @this
               .SetupExtension(new UseDependenciesTheoryExtension())
               .SetupExtension(new UseEventSourceImplementationTheoryExtension())
               .SetupExtension(new ConfigureDependenciesTheoryExtension())
               .SetupExtension(new PickDependencyTheoryExtension());
        }

        private static TheoryItem SetupExtensionsAsDelegate(
            this TheoryItem @this)
        {
            return @this
               .SetupExtension(new UseDependenciesTheoryExtension())
               .SetupExtension(new UseDelegateTheoryExtension())
               .SetupExtension(new ConfigureDependenciesTheoryExtension())
               .SetupExtension(new PickDependencyTheoryExtension());
        }
    }
}