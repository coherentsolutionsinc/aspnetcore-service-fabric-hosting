using System;
using System.Threading.Tasks;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

using IService = Microsoft.ServiceFabric.Services.Remoting.IService;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric
{
    public static class BuildersExtensions
    {
        public static Action<T> Chain<T>(
            this Action<T> left,
            Action<T> right)
        {
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            return v =>
            {
                left(v);
                right(v);
            };
        }

        public static TCaller UseDependencies<TCaller>(
            this TCaller @this,
            Func<IServiceCollection> factoryFunc)
            where TCaller : IConfigurableObject<IConfigurableObjectDependenciesConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDependencies(factoryFunc));

            return @this;
        }

        public static TCaller UseWebHostBuilder<TCaller>(
            this TCaller @this,
            Func<IWebHostBuilder> factoryFunc)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseWebHostBuilder(factoryFunc));

            return @this;
        }

        public static TCaller UseLoggerOptions<TCaller>(
            this TCaller @this,
            Func<IServiceHostLoggerOptions> factoryFunc)
            where TCaller : IConfigurableObject<IServiceHostLoggerConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseLoggerOptions(factoryFunc));

            return @this;
        }

        public static TCaller UseServiceType<TCaller>(
            this TCaller @this,
            string serviceName)
            where TCaller : IConfigurableObject<IServiceHostBuilderConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseServiceType(serviceName));

            return @this;
        }

        public static TCaller UseDelegateInvoker<TCaller>(
            this TCaller @this,
            Func<Delegate, IServiceProvider, IServiceHostDelegateInvoker> factoryFunc)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegateInvoker(factoryFunc));

            return @this;
        }

        public static TCaller UseDelegate<TCaller>(
            this TCaller @this,
            Action @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T>(
            this TCaller @this,
            Action<T> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2>(
            this TCaller @this,
            Action<T1, T2> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3>(
            this TCaller @this,
            Action<T1, T2, T3> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4>(
            this TCaller @this,
            Action<T1, T2, T3, T4> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5, T6> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5, T6, T7> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5, T6, T7, T8> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            this TCaller @this,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller>(
            this TCaller @this,
            Func<Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T>(
            this TCaller @this,
            Func<T, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2>(
            this TCaller @this,
            Func<T1, T2, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3>(
            this TCaller @this,
            Func<T1, T2, T3, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4>(
            this TCaller @this,
            Func<T1, T2, T3, T4, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, T6, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, T6, T7, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseDelegate<TCaller, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            this TCaller @this,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task> @delegate)
            where TCaller : IConfigurableObject<IServiceHostDelegateReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegate(@delegate));

            return @this;
        }

        public static TCaller UseEndpoint<TCaller>(
            this TCaller @this,
            string endpointName)
            where TCaller : IConfigurableObject<IServiceHostListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseEndpoint(endpointName));

            return @this;
        }

        public static TCaller UseKestrel<TCaller>(
            this TCaller @this,
            Action<KestrelServerOptions> configAction = null)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator =>
                {
                    configurator.UseCommunicationListener(
                        (
                            context,
                            endpoint,
                            factory) =>
                        {
                            return new KestrelCommunicationListener(context, endpoint, factory);
                        },
                        webHostBuilder =>
                        {
                            WebHostBuilderKestrelExtensions.UseKestrel(webHostBuilder, options => configAction?.Invoke(options));
                        });
                });

            return @this;
        }

        public static TCaller UseHttpSys<TCaller>(
            this TCaller @this,
            Action<HttpSysOptions> configAction = null)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator =>
                {
                    configurator.UseCommunicationListener(
                        (
                            context,
                            endpoint,
                            factory) =>
                        {
                            return new HttpSysCommunicationListener(context, endpoint, factory);
                        },
                        webHostBuilder =>
                        {
                            WebHostBuilderHttpSysExtensions.UseHttpSys(webHostBuilder, options => configAction?.Invoke(options));
                        });
                });

            return @this;
        }

        public static TCaller UseCommunicationListener<TCaller>(
            this TCaller @this,
            ServiceHostAspNetCoreCommunicationListenerFactory factoryFunc,
            Action<IWebHostBuilder> configAction)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseCommunicationListener(factoryFunc, configAction));

            return @this;
        }

        public static TCaller UseCommunicationListener<TCaller>(
            this TCaller @this,
            ServiceHostRemotingCommunicationListenerFactory factoryFunc)
            where TCaller : IConfigurableObject<IServiceHostRemotingListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseCommunicationListener(factoryFunc));

            return @this;
        }

        public static TCaller UseSettings<TCaller>(
            this TCaller @this,
            Func<FabricTransportRemotingListenerSettings> factoryFunc)
            where TCaller : IConfigurableObject<IServiceHostRemotingListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseSettings(factoryFunc));

            return @this;
        }

        public static TCaller UseUniqueServiceUrlIntegration<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator
                    => configurator.UseIntegrationOptions(ServiceFabricIntegrationOptions.UseUniqueServiceUrl));

            return @this;
        }

        public static TCaller UseReverseProxyIntegration<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator
                    => configurator.UseIntegrationOptions(ServiceFabricIntegrationOptions.UseReverseProxyIntegration));

            return @this;
        }

        public static TCaller UseListenerOnSecondary<TCaller>(
            this TCaller @this)
            where TCaller : IConfigurableObject<IStatefulServiceHostListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.UseListenerOnSecondary());

            return @this;
        }

        public static IStatefulServiceHostBuilder UseRuntimeRegistrant(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceRuntimeRegistrant> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseRuntimeRegistrant(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseRuntimeRegistrant(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceRuntimeRegistrant> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseRuntimeRegistrant(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostBuilder UseDelegateReplicaTemplate(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostDelegateReplicaTemplate> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegateReplicaTemplate(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseDelegateReplicaTemplate(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostDelegateReplicaTemplate> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegateReplicaTemplate(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostBuilder UseDelegateReplicator(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostDelegateReplicableTemplate, IStatefulServiceHostDelegateReplicator> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegateReplicator(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseDelegateReplicator(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostDelegateReplicableTemplate, IStatelessServiceHostDelegateReplicator> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseDelegateReplicator(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostBuilder UseAspNetCoreListenerReplicaTemplate(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostAspNetCoreListenerReplicaTemplate> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseAspNetCoreListenerReplicaTemplate(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseAspNetCoreListenerReplicaTemplate(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostAspNetCoreListenerReplicaTemplate> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseAspNetCoreListenerReplicaTemplate(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostBuilder UseRemotingListenerReplicaTemplate(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostRemotingListenerReplicaTemplate> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseRemotingListenerReplicaTemplate(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseRemotingListenerReplicaTemplate(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostRemotingListenerReplicaTemplate> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseRemotingListenerReplicaTemplate(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostBuilder UseListenerReplicator(
            this IStatefulServiceHostBuilder @this,
            Func<IStatefulServiceHostListenerReplicableTemplate, IStatefulServiceHostListenerReplicator> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseListenerReplicator(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostBuilder UseListenerReplicator(
            this IStatelessServiceHostBuilder @this,
            Func<IStatelessServiceHostListenerReplicableTemplate, IStatelessServiceHostListenerReplicator> factoryFunc)
        {
            @this.ConfigureObject(
                configurator => configurator.UseListenerReplicator(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation<TRemotingImplementation>(null));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation<TRemotingImplementation>(null));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Func<TRemotingImplementation> factoryFunc)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation(provider => factoryFunc()));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Func<TRemotingImplementation> factoryFunc)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation(provider => factoryFunc()));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Func<IServiceProvider, TRemotingImplementation> factoryFunc)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseImplementation<TRemotingImplementation>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Func<IServiceProvider, TRemotingImplementation> factoryFunc)
            where TRemotingImplementation : IService
        {
            @this.ConfigureObject(
                configurator => configurator.UseImplementation(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseSerializationProvider<TSerializationProvider>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this)
            where TSerializationProvider : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializationProvider<TSerializationProvider>(null));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseSerializationProvider<TSerializationProvider>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this)
            where TSerializationProvider : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializationProvider<TSerializationProvider>(null));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseSerializationProvider<TSerializationProvider>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Func<TSerializationProvider> factoryFunc)
            where TSerializationProvider : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializationProvider(provider => factoryFunc()));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseSerializationProvider<TSerializationProvider>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Func<TSerializationProvider> factoryFunc)
            where TSerializationProvider : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializationProvider(provider => factoryFunc()));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseSerializationProvider<TSerializationProvider>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Func<IServiceProvider, TSerializationProvider> factoryFunc)
            where TSerializationProvider : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializationProvider(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseSerializationProvider<TSerializationProvider>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Func<IServiceProvider, TSerializationProvider> factoryFunc)
            where TSerializationProvider : IServiceRemotingMessageSerializationProvider
        {
            @this.ConfigureObject(
                configurator => configurator.UseSerializationProvider(factoryFunc));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseHandler<THandler>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this)
            where THandler : IServiceRemotingMessageHandler
        {
            @this.ConfigureObject(
                configurator => configurator.UseHandler<THandler>(null));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseHandler<THandler>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this)
            where THandler : IServiceRemotingMessageHandler
        {
            @this.ConfigureObject(
                configurator => configurator.UseHandler<THandler>(null));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseHandler<THandler>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Func<THandler> factoryFunc)
            where THandler : IServiceRemotingMessageHandler
        {
            @this.ConfigureObject(
                configurator => configurator.UseHandler(provider => factoryFunc()));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseHandler<THandler>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Func<THandler> factoryFunc)
            where THandler : IServiceRemotingMessageHandler
        {
            @this.ConfigureObject(
                configurator => configurator.UseHandler(provider => factoryFunc()));

            return @this;
        }

        public static IStatefulServiceHostRemotingListenerReplicaTemplate UseHandler<THandler>(
            this IStatefulServiceHostRemotingListenerReplicaTemplate @this,
            Func<IServiceProvider, THandler> factoryFunc)
            where THandler : IServiceRemotingMessageHandler
        {
            @this.ConfigureObject(
                configurator => configurator.UseHandler(factoryFunc));

            return @this;
        }

        public static IStatelessServiceHostRemotingListenerReplicaTemplate UseHandler<THandler>(
            this IStatelessServiceHostRemotingListenerReplicaTemplate @this,
            Func<IServiceProvider, THandler> factoryFunc)
            where THandler : IServiceRemotingMessageHandler
        {
            @this.ConfigureObject(
                configurator => configurator.UseHandler(factoryFunc));

            return @this;
        }

        public static TCaller ConfigureDependencies<TCaller>(
            this TCaller @this,
            Action<IServiceCollection> configAction)
            where TCaller : IConfigurableObject<IConfigurableObjectDependenciesConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureDependencies(configAction));

            return @this;
        }

        public static TCaller ConfigureWebHost<TCaller>(
            this TCaller @this,
            Action<IWebHostBuilder> configAction)
            where TCaller : IConfigurableObject<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        {
            @this.ConfigureObject(
                configurator => configurator.ConfigureWebHost(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder DefineDelegate(
            this IStatefulServiceHostBuilder @this,
            Action<IStatefulServiceHostDelegateReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineDelegate(configAction));

            return @this;
        }

        public static IStatelessServiceHostBuilder DefineDelegate(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostDelegateReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineDelegate(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder DefineAspNetCoreListener(
            this IStatefulServiceHostBuilder @this,
            Action<IStatefulServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineAspNetCoreListener(configAction));

            return @this;
        }

        public static IStatefulServiceHostBuilder DefineRemotingListener(
            this IStatefulServiceHostBuilder @this,
            Action<IStatefulServiceHostRemotingListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineRemotingListener(configAction));

            return @this;
        }

        public static IStatelessServiceHostBuilder DefineAspNetCoreListener(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostAspNetCoreListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineAspNetCoreListener(configAction));

            return @this;
        }

        public static IStatelessServiceHostBuilder DefineRemotingListener(
            this IStatelessServiceHostBuilder @this,
            Action<IStatelessServiceHostRemotingListenerReplicaTemplate> configAction)
        {
            @this.ConfigureObject(
                configurator => configurator.DefineRemotingListener(configAction));

            return @this;
        }
    }
}