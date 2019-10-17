using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        private static class DelegateWrap
        {
            public static Delegate Create(
                Delegate source,
                Delegate hook)
            {
                var sourceParameters = source.Method.GetParameters();
                var sourceDynamicInvokeMethod = source.GetType().GetMethod(nameof(source.DynamicInvoke));

                var hookParameters = hook.Method.GetParameters();
                var hookDynamicInvokeMethod = source.GetType().GetMethod(nameof(hook.DynamicInvoke));

                var sourceParameterExpressions = sourceParameters
                   .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                   .ToArray();

                var hookParameterExpressions = hookParameters
                   .GroupJoin(
                        sourceParameterExpressions,
                        p => p.ParameterType,
                        p => p.Type,
                        (
                            p,
                            expressions) =>
                        {
                            var expression = expressions.FirstOrDefault() ?? Expression.Parameter(p.ParameterType, p.Name);

                            return expression;
                        })
                   .ToArray();

                var sourceInvocationExpression = Expression.Call(
                    Expression.Constant(source),
                    sourceDynamicInvokeMethod,
                    Expression.NewArrayInit(typeof(object), sourceParameterExpressions));

                var hookInvocationExpression = Expression.Call(
                    Expression.Constant(hook),
                    hookDynamicInvokeMethod,
                    Expression.NewArrayInit(typeof(object), hookParameterExpressions));

                return Expression.Lambda(
                        Expression.Block(hookInvocationExpression, sourceInvocationExpression),
                        sourceParameterExpressions.Union(hookParameterExpressions))
                   .Compile();
            }
        }

        private class ServiceDelegateInvokerDecorator : IServiceDelegateInvoker
        {
            private readonly IEnumerable<Action<IServiceProvider>> pickActions;
            private readonly IServiceDelegateInvoker target;

            public ServiceDelegateInvokerDecorator(
                IEnumerable<Action<IServiceProvider>> pickActions,
                IServiceDelegateInvoker target)
            {
                this.pickActions = pickActions 
                    ?? throw new ArgumentNullException(nameof(pickActions));

                this.target = target 
                    ?? throw new ArgumentNullException(nameof(target));
            }

            public Task InvokeAsync(
                Delegate @delegate,
                IServiceDelegateInvocationContext invocationContext,
                CancellationToken cancellationToken)
            {
                @delegate = DelegateWrap.Create(
                    @delegate,
                    new Action<IServiceProvider>(
                        serviceProvider =>
                        {
                            foreach (var pickAction in this.pickActions)
                            {
                                pickAction(serviceProvider);
                            }
                        }));

                var result = this.target.InvokeAsync(@delegate, invocationContext, cancellationToken);

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

        public static void ConfigureEventSourceExtensions(
            IStatefulServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.SetupEventSource(
                eventSourceBuilder =>
                {
                    eventSourceBuilder.ConfigureObject(c => ConfigureEventSourceExtensions(c, extensions));
                });
        }

        public static void ConfigureEventSourceExtensions(
            IStatelessServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.SetupEventSource(
                eventSourceBuilder =>
                {
                    eventSourceBuilder.ConfigureObject(c => ConfigureEventSourceExtensions(c, extensions));
                });
        }

        public static void ConfigureDelegateExtensions(
            IStatefulServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.DefineDelegate(
                delegateBuilder =>
                {
                    delegateBuilder.ConfigureObject(c => ConfigureDelegateExtensions(c, extensions));
                    delegateBuilder.ConfigureObject(
                        c =>
                        {
                            var useDelegateEvent = extensions
                               .GetExtension<IUseDelegateEventTheoryExtension<StatefulServiceLifecycleEvent>>();

                            var useDelegateInvoker = extensions
                               .GetExtension<IUseDelegateInvokerTheoryExtension>();

                            var pickDependency = extensions.GetExtension<IPickDependencyTheoryExtension>();

                            c.UseEvent(useDelegateEvent.Event);
                            c.UseDelegateInvoker(provider => new ServiceDelegateInvokerDecorator(
                                pickDependency.PickActions, 
                                useDelegateInvoker.Factory(provider)));
                        });
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
                    delegateBuilder.ConfigureObject(
                        c =>
                        {
                            var useDelegateEvent = extensions
                               .GetExtension<IUseDelegateEventTheoryExtension<StatelessServiceLifecycleEvent>>();

                            var useDelegateInvoker = extensions
                               .GetExtension<IUseDelegateInvokerTheoryExtension>();

                            var pickDependency = extensions.GetExtension<IPickDependencyTheoryExtension>();

                            c.UseEvent(useDelegateEvent.Event);
                            c.UseDelegateInvoker(provider => new ServiceDelegateInvokerDecorator(
                                pickDependency.PickActions,
                                useDelegateInvoker.Factory(provider)));
                        });
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

        public static void ConfigureGenericListenerExtensions(
            IStatefulServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.DefineGenericListener(
                listenerBuilder =>
                {
                    listenerBuilder.ConfigureObject(c => ConfigureGenericListenerExtensions(c, extensions));
                });
        }

        public static void ConfigureGenericListenerExtensions(
            IStatelessServiceHostBuilderConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            configurator.DefineGenericListener(
                listenerBuilder =>
                {
                    listenerBuilder.ConfigureObject(c => ConfigureGenericListenerExtensions(c, extensions));
                });
        }

        private static void ConfigureEventSourceExtensions(
            IServiceHostEventSourceReplicaTemplateConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            var useDependencies = extensions.GetExtension<IUseDependenciesTheoryExtension>();
            var useEventSourceImplementation = extensions.GetExtension<IUseEventSourceImplementationTheoryExtension>();
            var configureDependencies = extensions.GetExtension<IConfigureDependenciesTheoryExtension>();
            var pickDependency = extensions.GetExtension<IPickDependencyTheoryExtension>();

            configurator.UseDependencies(useDependencies.Factory);
            configurator.ConfigureDependencies(
                dependencies =>
                {
                    configureDependencies.ConfigAction(dependencies);
                });
            configurator.UseImplementation(
                provider =>
                {
                    foreach (var action in pickDependency.PickActions)
                    {
                        action(provider);
                    }

                    return useEventSourceImplementation.Factory(provider);
                });
        }

        private static void ConfigureDelegateExtensions(
            IServiceHostDelegateReplicaTemplateConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            var useDelegate = extensions.GetExtension<IUseDelegateTheoryExtension>();
            var useDependencies = extensions.GetExtension<IUseDependenciesTheoryExtension>();
            var configureDependencies = extensions.GetExtension<IConfigureDependenciesTheoryExtension>();

            configurator.UseDependencies(useDependencies.Factory);
            configurator.UseDelegate(useDelegate.Delegate);
            configurator.ConfigureDependencies(
                dependencies =>
                {
                    configureDependencies.ConfigAction(dependencies);
                });
        }

        private static void ConfigureAspNetCoreListenerExtensions(
            IServiceHostAspNetCoreListenerReplicaTemplateConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            var useListenerEndpoint = extensions.GetExtension<IUseListenerEndpointTheoryExtension>();
            var useAspNetCoreListenerCommunicationListener = extensions.GetExtension<IUseAspNetCoreListenerCommunicationListenerTheoryExtension>();
            var useAspNetCoreWebHostBuilder = extensions.GetExtension<IUseAspNetCoreListenerWebHostBuilderTheoryExtension>();
            var configureDependencies = extensions.GetExtension<IConfigureDependenciesTheoryExtension>();
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
            configurator.ConfigureWebHost(
                webHostBuilder =>
                {
                    webHostBuilder.ConfigureServices(
                        dependencies =>
                        {
                            configureDependencies.ConfigAction(dependencies);
                        });
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
            var configureDependencies = extensions.GetExtension<IConfigureDependenciesTheoryExtension>();
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
            configurator.ConfigureDependencies(
                dependencies =>
                {
                    configureDependencies.ConfigAction(dependencies);
                });
        }

        private static void ConfigureGenericListenerExtensions(
            IServiceHostGenericListenerReplicaTemplateConfigurator configurator,
            TheoryItem.TheoryItemExtensionProvider extensions)
        {
            var useListenerEndpoint = extensions.GetExtension<IUseListenerEndpointTheoryExtension>();
            var useGenericCommunicationListener = extensions.GetExtension<IUseGenericListenerCommunicationListenerTheoryExtension>();
            var useDependencies = extensions.GetExtension<IUseDependenciesTheoryExtension>();
            var configureDependencies = extensions.GetExtension<IConfigureDependenciesTheoryExtension>();
            var pickDependency = extensions.GetExtension<IPickDependencyTheoryExtension>();
            var pickListenerEndpoint = extensions.GetExtension<IPickListenerEndpointTheoryExtension>();

            configurator.UseEndpoint(useListenerEndpoint.Endpoint);
            configurator.UseDependencies(useDependencies.Factory);
            configurator.UseCommunicationListener(
                (
                    context,
                    endpointName,
                    provider) =>
                {
                    pickListenerEndpoint.PickAction(endpointName);

                    foreach (var pickAction in pickDependency.PickActions)
                    {
                        pickAction(provider);
                    }

                    return useGenericCommunicationListener.Factory(context, endpointName, provider);
                });
            configurator.ConfigureDependencies(
                dependencies =>
                {
                    configureDependencies.ConfigAction(dependencies);
                });
        }
    }
}