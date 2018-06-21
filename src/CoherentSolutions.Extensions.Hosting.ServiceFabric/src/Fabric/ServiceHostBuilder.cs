using System;
using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostBuilder<
            TServiceHost,
            TParameters,
            TConfigurator,
            TAsyncDelegateReplicableTemplate,
            TAsyncDelegateReplicaTemplate,
            TAsyncDelegateReplicator,
            TListenerReplicableTemplate,
            TListenerAspNetCoreReplicaTemplate,
            TListenerRemotingReplicaTemplate,
            TListenerReplicator>
        : ConfigurableObject<TConfigurator>,
          IServiceHostBuilder<TServiceHost, TConfigurator>
        where TParameters :
        IServiceHostBuilderParameters,
        IServiceHostBuilderDelegateParameters<TAsyncDelegateReplicaTemplate>,
        IServiceHostBuilderDelegateReplicationParameters<TAsyncDelegateReplicableTemplate, TAsyncDelegateReplicator>,
        IServiceHostBuilderAspNetCoreListenerParameters<TListenerAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerParameters<TListenerRemotingReplicaTemplate>,
        IServiceHostBuilderListenerReplicationParameters<TListenerReplicableTemplate, TListenerReplicator>
        where TConfigurator :
        IServiceHostBuilderConfigurator,
        IServiceHostBuilderDelegateConfigurator<TAsyncDelegateReplicaTemplate>,
        IServiceHostBuilderDelegateReplicationConfigurator<TAsyncDelegateReplicableTemplate, TAsyncDelegateReplicator>,
        IServiceHostBuilderAspNetCoreListenerConfigurator<TListenerAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerConfigurator<TListenerRemotingReplicaTemplate>,
        IServiceHostBuilderListenerReplicationConfigurator<TListenerReplicableTemplate, TListenerReplicator>
        where TAsyncDelegateReplicaTemplate :
        TAsyncDelegateReplicableTemplate,
        IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>
        where TListenerAspNetCoreReplicaTemplate :
        TListenerReplicableTemplate,
        IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        where TListenerRemotingReplicaTemplate :
        TListenerReplicableTemplate,
        IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>
    {
        protected abstract class Parameters
            : IServiceHostBuilderParameters,
              IServiceHostBuilderConfigurator,
              IServiceHostBuilderDelegateParameters<TAsyncDelegateReplicaTemplate>,
              IServiceHostBuilderDelegateConfigurator<TAsyncDelegateReplicaTemplate>,
              IServiceHostBuilderDelegateReplicationParameters<TAsyncDelegateReplicableTemplate, TAsyncDelegateReplicator>,
              IServiceHostBuilderDelegateReplicationConfigurator<TAsyncDelegateReplicableTemplate, TAsyncDelegateReplicator>,
              IServiceHostBuilderAspNetCoreListenerParameters<TListenerAspNetCoreReplicaTemplate>,
              IServiceHostBuilderAspNetCoreListenerConfigurator<TListenerAspNetCoreReplicaTemplate>,
              IServiceHostBuilderRemotingListenerParameters<TListenerRemotingReplicaTemplate>,
              IServiceHostBuilderRemotingListenerConfigurator<TListenerRemotingReplicaTemplate>,
              IServiceHostBuilderListenerReplicationParameters<TListenerReplicableTemplate, TListenerReplicator>,
              IServiceHostBuilderListenerReplicationConfigurator<TListenerReplicableTemplate, TListenerReplicator>
        {
            public string ServiceTypeName { get; private set; }

            public List<IServiceHostListenerDescriptor> ListenerDescriptors { get; private set; }

            public List<IServiceHostDelegateDescriptor> DelegateDescriptors { get; private set; }

            public Func<IServiceHostDelegateInvoker> DelegateInvokerFunc { get; private set; }

            public Func<TAsyncDelegateReplicaTemplate> DelegateReplicaTemplateFunc { get; private set; }

            public Func<TAsyncDelegateReplicableTemplate, TAsyncDelegateReplicator> DelegateReplicatorFunc { get; private set; }

            public Func<TListenerAspNetCoreReplicaTemplate> AspNetCoreListenerReplicaTemplateFunc { get; private set; }

            public Func<TListenerRemotingReplicaTemplate> RemotingListenerReplicaTemplateFunc { get; private set; }

            public Func<TListenerReplicableTemplate, TListenerReplicator> ListenerReplicatorFunc { get; private set; }

            public Func<IServiceCollection> DependenciesFunc { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            protected Parameters()
            {
                this.ServiceTypeName = string.Empty;
                this.ListenerDescriptors = null;
                this.DelegateDescriptors = null;
                this.DelegateInvokerFunc = DefaulDelegateInvokerFunc;
                this.DelegateReplicaTemplateFunc = null;
                this.DelegateReplicatorFunc = null;
                this.AspNetCoreListenerReplicaTemplateFunc = null;
                this.RemotingListenerReplicaTemplateFunc = null;
                this.ListenerReplicatorFunc = null;
                this.DependenciesFunc = DefaulDependenciesFunc;
                this.DependenciesConfigAction = null;
            }

            public void UseServiceType(
                string serviceName)
            {
                this.ServiceTypeName = serviceName
                 ?? throw new ArgumentNullException(nameof(serviceName));
            }

            public void UseDelegateInvoker(
                Func<IServiceHostDelegateInvoker> factoryFunc)
            {
                this.DelegateInvokerFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseDelegateReplicaTemplate(
                Func<TAsyncDelegateReplicaTemplate> factoryFunc)
            {
                this.DelegateReplicaTemplateFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseDelegateReplicator(
                Func<TAsyncDelegateReplicableTemplate, TAsyncDelegateReplicator> factoryFunc)
            {
                this.DelegateReplicatorFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseAspNetCoreListenerReplicaTemplate(
                Func<TListenerAspNetCoreReplicaTemplate> factoryFunc)
            {
                this.AspNetCoreListenerReplicaTemplateFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseRemotingListenerReplicaTemplate(
                Func<TListenerRemotingReplicaTemplate> factoryFunc)
            {
                this.RemotingListenerReplicaTemplateFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseListenerReplicator(
                Func<TListenerReplicableTemplate, TListenerReplicator> factoryFunc)
            {
                this.ListenerReplicatorFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseDependencies(
                Func<IServiceCollection> factoryFunc)
            {
                this.DependenciesFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void ConfigureDependencies(
                Action<IServiceCollection> configAction)
            {
                if (configAction == null)
                {
                    throw new ArgumentNullException(nameof(configAction));
                }

                this.DependenciesConfigAction = this.DependenciesConfigAction.Chain(configAction);
            }

            public void DefineAsyncDelegate(
                Action<TAsyncDelegateReplicaTemplate> defineAction)
            {
                if (defineAction == null)
                {
                    throw new ArgumentNullException(nameof(defineAction));
                }

                if (this.DelegateDescriptors == null)
                {
                    this.DelegateDescriptors = new List<IServiceHostDelegateDescriptor>();
                }

                this.DelegateDescriptors.Add(
                    new ServiceHostDelegateDescriptor(
                        replicaTemplate => defineAction((TAsyncDelegateReplicaTemplate) replicaTemplate)));
            }

            public void DefineAspNetCoreListener(
                Action<TListenerAspNetCoreReplicaTemplate> defineAction)
            {
                if (defineAction == null)
                {
                    throw new ArgumentNullException(nameof(defineAction));
                }

                if (this.ListenerDescriptors == null)
                {
                    this.ListenerDescriptors = new List<IServiceHostListenerDescriptor>();
                }

                this.ListenerDescriptors.Add(
                    new ServiceHostListenerDescriptor(
                        ServiceHostListenerType.AspNetCore,
                        replicaTemplate => defineAction((TListenerAspNetCoreReplicaTemplate) replicaTemplate)));
            }

            public void DefineRemotingListener(
                Action<TListenerRemotingReplicaTemplate> defineAction)
            {
                if (defineAction == null)
                {
                    throw new ArgumentNullException(nameof(defineAction));
                }

                if (this.ListenerDescriptors == null)
                {
                    this.ListenerDescriptors = new List<IServiceHostListenerDescriptor>();
                }

                this.ListenerDescriptors.Add(
                    new ServiceHostListenerDescriptor(
                        ServiceHostListenerType.Remoting,
                        replicaTemplate => defineAction((TListenerRemotingReplicaTemplate) replicaTemplate)));
            }

            public static IServiceHostDelegateInvoker DefaulDelegateInvokerFunc()
            {
                return new ServiceHostDelegateInvoker();
            }

            public static IServiceCollection DefaulDependenciesFunc()
            {
                return new ServiceCollection();
            }
        }

        protected class Compilation
        {
            public IServiceHostDelegateInvoker DelegateInvoker { get; }

            public IReadOnlyList<TAsyncDelegateReplicator> DelegateReplicators { get; }

            public IReadOnlyList<TListenerReplicator> ListenerReplicators { get; }

            public Compilation(
                IServiceHostDelegateInvoker delegateInvoker,
                IReadOnlyList<TAsyncDelegateReplicator> delegateReplicators,
                IReadOnlyList<TListenerReplicator> listenerReplicators)
            {
                this.DelegateInvoker = delegateInvoker
                 ?? throw new ArgumentNullException(nameof(delegateInvoker));

                this.DelegateReplicators = delegateReplicators
                 ?? throw new ArgumentNullException(nameof(delegateReplicators));

                this.ListenerReplicators = listenerReplicators
                 ?? throw new ArgumentNullException(nameof(listenerReplicators));
            }
        }

        public abstract TServiceHost Build();

        protected Compilation CompileParameters(
            TParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.ListenerReplicatorFunc == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(parameters.ListenerReplicatorFunc)} was configured");
            }

            // Initialize service dependencies
            var dependenciesCollection = parameters.DependenciesFunc();
            if (dependenciesCollection == null)
            {
                throw new FactoryProducesNullInstanceException<IServiceHostDelegateInvoker>();
            }

            parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

            var dependencies = new DefaultServiceProviderFactory().CreateServiceProvider(dependenciesCollection);

            var delegateInvoker = parameters.DelegateInvokerFunc();
            if (delegateInvoker == null)
            {
                throw new FactoryProducesNullInstanceException<IServiceHostDelegateInvoker>();
            }

            var delegateReplicators = parameters.DelegateDescriptors == null
                ? Array.Empty<TAsyncDelegateReplicator>()
                : parameters.DelegateDescriptors
                   .Select(
                        descriptor =>
                        {
                            if (parameters.DelegateReplicaTemplateFunc == null)
                            {
                                throw new InvalidOperationException(
                                    $"No {nameof(parameters.DelegateReplicaTemplateFunc)} was configured");
                            }

                            var template = parameters.DelegateReplicaTemplateFunc();
                            if (template == null)
                            {
                                throw new FactoryProducesNullInstanceException<TAsyncDelegateReplicaTemplate>();
                            }

                            template.ConfigureObject(
                                c =>
                                {
                                    c.ConfigureDependencies(
                                        services =>
                                        {
                                            DependencyRegistrant.Register(services, dependenciesCollection, dependencies);
                                        });
                                });

                            descriptor.ConfigAction(template);

                            var replicator = parameters.DelegateReplicatorFunc(template);
                            if (replicator == null)
                            {
                                throw new FactoryProducesNullInstanceException<TAsyncDelegateReplicator>();
                            }

                            return replicator;
                        })
                   .ToArray();

            var listenerReplicators = parameters.ListenerDescriptors == null
                ? Array.Empty<TListenerReplicator>()
                : parameters.ListenerDescriptors
                   .Select(
                        descriptor =>
                        {
                            TListenerReplicableTemplate replicableTemplate;
                            switch (descriptor.ListenerType)
                            {
                                case ServiceHostListenerType.AspNetCore:
                                    {
                                        if (parameters.AspNetCoreListenerReplicaTemplateFunc == null)
                                        {
                                            throw new InvalidOperationException(
                                                $"No {nameof(parameters.AspNetCoreListenerReplicaTemplateFunc)} was configured");
                                        }

                                        var template = parameters.AspNetCoreListenerReplicaTemplateFunc();
                                        if (template == null)
                                        {
                                            throw new FactoryProducesNullInstanceException<TListenerAspNetCoreReplicaTemplate>();
                                        }

                                        template.ConfigureObject(
                                            c =>
                                            {
                                                c.ConfigureWebHost(
                                                    builder =>
                                                    {
                                                        builder.ConfigureServices(
                                                            services =>
                                                            {
                                                                DependencyRegistrant.Register(services, dependenciesCollection, dependencies);
                                                            });
                                                    });
                                            });

                                        descriptor.ConfigAction(template);

                                        replicableTemplate = template;
                                    }
                                    break;
                                case ServiceHostListenerType.Remoting:
                                    {
                                        if (parameters.RemotingListenerReplicaTemplateFunc == null)
                                        {
                                            throw new InvalidOperationException(
                                                $"No {nameof(parameters.RemotingListenerReplicaTemplateFunc)} was configured");
                                        }

                                        var template = parameters.RemotingListenerReplicaTemplateFunc();
                                        if (template == null)
                                        {
                                            throw new FactoryProducesNullInstanceException<TListenerRemotingReplicaTemplate>();
                                        }

                                        template.ConfigureObject(
                                            c =>
                                            {
                                                c.ConfigureDependencies(
                                                    services =>
                                                    {
                                                        DependencyRegistrant.Register(services, dependenciesCollection, dependencies);
                                                    });
                                            });

                                        descriptor.ConfigAction(template);

                                        replicableTemplate = template;
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(descriptor.ListenerType));
                            }

                            var replicator = parameters.ListenerReplicatorFunc(replicableTemplate);
                            if (replicator == null)
                            {
                                throw new FactoryProducesNullInstanceException<TListenerReplicator>();
                            }

                            return replicator;
                        })
                   .ToArray();

            return new Compilation(delegateInvoker, delegateReplicators, listenerReplicators);
        }
    }
}