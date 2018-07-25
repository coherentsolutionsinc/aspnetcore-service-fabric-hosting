using System;
using System.Threading;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Extensions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Tests.Theories.Items
{
    public static class TheoryItemConfigure
    {
        private class ServiceHostDelegateInvokerDecorator : IServiceHostDelegateInvoker
        {
            private readonly IServiceHostDelegateInvoker target;

            private readonly Action beforeInvoke;

            private readonly Action afterInvoke;

            public ServiceHostDelegateInvokerDecorator(
                IServiceHostDelegateInvoker target,
                Action beforeInvoke = null,
                Action afterInvoke = null)
            {
                this.target = target;
                this.beforeInvoke = beforeInvoke;
                this.afterInvoke = afterInvoke;
            }

            public Task InvokeAsync(
                CancellationToken cancellationToken)
            {
                this.beforeInvoke?.Invoke();

                var result = this.target.InvokeAsync(cancellationToken);

                this.afterInvoke?.Invoke();

                return result;
            }
        }

        private class WebHostBuilderDecorator : IWebHostBuilder
        {
            private readonly IWebHostBuilder implementation;

            private readonly Action beforeBuild;

            private readonly Action<IWebHost> afterBuild;

            public WebHostBuilderDecorator(
                IWebHostBuilder implementation,
                Action beforeBuild = null,
                Action<IWebHost> afterBuild = null)
            {
                this.implementation = implementation;
                this.beforeBuild = beforeBuild;
                this.afterBuild = afterBuild;
            }

            public IWebHost Build()
            {
                this.beforeBuild?.Invoke();

                var host = this.implementation.Build();

                this.afterBuild?.Invoke(host);

                return host;
            }

            public IWebHostBuilder ConfigureAppConfiguration(
                Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
            {
                return this.implementation.ConfigureAppConfiguration(configureDelegate);
            }

            public IWebHostBuilder ConfigureServices(
                Action<IServiceCollection> configureServices)
            {
                return this.implementation.ConfigureServices(configureServices);
            }

            public IWebHostBuilder ConfigureServices(
                Action<WebHostBuilderContext, IServiceCollection> configureServices)
            {
                return this.implementation.ConfigureServices(configureServices);
            }

            public string GetSetting(
                string key)
            {
                return this.implementation.GetSetting(key);
            }

            public IWebHostBuilder UseSetting(
                string key,
                string value)
            {
                return this.implementation.UseSetting(key, value);
            }
        }

        public static void ConfigureDelegateExtensions(
            IStatefulServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.DefineDelegate(
                delegateBuilder =>
                {
                    delegateBuilder.ConfigureObject(c => ConfigureDelegateExtensions(c, extensions));
                });
        }

        public static void ConfigureDelegateExtensions(
            IStatelessServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.DefineDelegate(
                delegateBuilder =>
                {
                    delegateBuilder.ConfigureObject(c => ConfigureDelegateExtensions(c, extensions));
                });
        }

        public static void ConfigureAspNetCoreListenerExtensions(
            IStatefulServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.DefineAspNetCoreListener(
                listenerBuilder =>
                {
                    listenerBuilder.ConfigureObject(c => ConfigureAspNetCoreListenerExtensions(c, extensions));
                });
        }

        public static void ConfigureAspNetCoreListenerExtensions(
            IStatelessServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.DefineAspNetCoreListener(
                listenerBuilder =>
                {
                    listenerBuilder.ConfigureObject(c => ConfigureAspNetCoreListenerExtensions(c, extensions));
                });
        }

        public static void ConfigureRemotingListenerExtensions(
            IStatefulServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.DefineRemotingListener(
                listenerBuilder =>
                {
                    listenerBuilder.ConfigureObject(c => ConfigureRemotingListenerExtensions(c, extensions));
                });
        }

        public static void ConfigureRemotingListenerExtensions(
            IStatelessServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.DefineRemotingListener(
                listenerBuilder =>
                {
                    listenerBuilder.ConfigureObject(c => ConfigureRemotingListenerExtensions(c, extensions));
                });
        }

        private static void ConfigureDelegateExtensions(
            IServiceHostDelegateReplicaTemplateConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            var useDelegate = extensions.GetExtension<IUseDelegateTheoryExtension>();
            var useDelegateInvoker = extensions.GetExtension<IUseDelegateInvokerTheoryExtension>();
            var useDependencies = extensions.GetExtension<IUseDependenciesTheoryExtension>();
            var pickDependency = extensions.GetExtension<IPickDependencyTheoryExtension>();

            configurator.UseDependencies(useDependencies.Factory);
            configurator.UseDelegate(useDelegate.Delegate);
            configurator.UseDelegateInvoker(
                (
                    @delegate,
                    provider) =>
                {
                    var invoker = useDelegateInvoker.Factory(@delegate, provider);
                    return new ServiceHostDelegateInvokerDecorator(
                        invoker,
                        beforeInvoke: () =>
                        {
                            foreach (var pickAction in pickDependency.PickActions)
                            {
                                pickAction(provider);
                            }
                        });
                });
        }

        private static void ConfigureAspNetCoreListenerExtensions(
            IServiceHostAspNetCoreListenerReplicaTemplateConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            var useListenerEndpoint = extensions.GetExtension<IUseListenerEndpointTheoryExtension>();
            var useAspNetCoreListenerCommunicationListener = extensions.GetExtension<IUseAspNetCoreListenerCommunicationListenerTheoryExtension>();
            var useAspNetCoreWebHostBuilder = extensions.GetExtension<IUseAspNetCoreListenerWebHostBuilderTheoryExtension>();
            var pickDependency = extensions.GetExtension<IPickDependencyTheoryExtension>();
            var pickListenerEndpoint = extensions.GetExtension<IPickListenerEndpointTheoryExtension>();

            configurator.UseEndpoint(useListenerEndpoint.Endpoint);
            configurator.UseWebHostBuilder(
                () => new WebHostBuilderDecorator(
                    useAspNetCoreWebHostBuilder.Factory(),
                    afterBuild: host =>
                    {
                        foreach (var pickAction in pickDependency.PickActions)
                        {
                            pickAction(host.Services);
                        }
                    }));
            configurator.UseCommunicationListener(
                (
                    context,
                    endpointName,
                    factory) =>
                {
                    pickListenerEndpoint.PickAction(endpointName);

                    return useAspNetCoreListenerCommunicationListener.Factory(context, endpointName, factory);
                },
                builder =>
                {
                });
        }

        private static void ConfigureRemotingListenerExtensions(
            IServiceHostRemotingListenerReplicaTemplateConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            var useListenerEndpoint = extensions.GetExtension<IUseListenerEndpointTheoryExtension>();
            var useRemotingCommunicationListener = extensions.GetExtension<IUseRemotingListenerCommunicationListenerTheoryExtension>();
            var useRemotingImplementation = extensions.GetExtension<IUseRemotingListenerImplementationTheoryExtension>();
            var useRemotingSettings = extensions.GetExtension<IUseRemotingListenerSettingsTheoryExtension>();
            var useRemotingSerializationProvider = extensions.GetExtension<IUseRemotingListenerSerializerTheoryExtension>();
            var useRemotingHandler = extensions.GetExtension<IUseRemotingListenerHandlerTheoryExtension>();
            var useDependencies = extensions.GetExtension<IUseDependenciesTheoryExtension>();
            var pickDependency = extensions.GetExtension<IPickDependencyTheoryExtension>();
            var pickListenerEndpoint = extensions.GetExtension<IPickListenerEndpointTheoryExtension>();
            var pickRemotingImplementation = extensions.GetExtension<IPickRemotingListenerImplementationTheoryExtension>();
            var pickRemotingSettings = extensions.GetExtension<IPickRemotingListenerSettingsTheoryExtension>();
            var pickRemotingSerializationProvider = extensions.GetExtension<IPickRemotingListenerSerializationProviderTheoryExtension>();
            var pickRemotingHandler = extensions.GetExtension<IPickRemotingListenerHandlerTheoryExtension>();

            configurator.UseEndpoint(useListenerEndpoint.Endpoint);
            configurator.UseDependencies(useDependencies.Factory);
            configurator.UseHandler(
                provider =>
                {
                    pickRemotingImplementation.PickAction(provider.GetService<IService>());

                    var handler = useRemotingHandler.Factory(provider);

                    pickRemotingHandler.PickAction(handler);

                    return handler;
                });
            configurator.UseCommunicationListener(
                (
                    context,
                    build) =>
                {
                    var options = build(context);

                    pickListenerEndpoint.PickAction(options.ListenerSettings.EndpointResourceName);
                    pickRemotingSerializationProvider.PickAction(options.MessageSerializationProvider);
                    pickRemotingSettings.PickAction(options.ListenerSettings);

                    return useRemotingCommunicationListener.Factory(context, build);
                });
            configurator.UseImplementation(
                provider =>
                {
                    foreach (var action in pickDependency.PickActions)
                    {
                        action(provider);
                    }

                    return useRemotingImplementation.Factory(provider);
                });
            configurator.UseSerializationProvider(useRemotingSerializationProvider.Factory);
            configurator.UseSettings(useRemotingSettings.Factory);
        }
    }
}