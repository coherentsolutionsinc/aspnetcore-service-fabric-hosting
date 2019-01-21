using System;
using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostBuilder<
            TServiceHost,
            TParameters,
            TConfigurator,
            TEventSourceReplicableTemplate,
            TEventSourceReplicaTemplate,
            TEventSourceReplicator,
            TDelegateReplicableTemplate,
            TDelegateReplicaTemplate,
            TDelegateReplicator,
            TListenerReplicableTemplate,
            TListenerAspNetCoreReplicaTemplate,
            TListenerRemotingReplicaTemplate,
            TListenerGenericReplicaTemplate,
            TListenerReplicator>
        : ConfigurableObject<TConfigurator>,
          IServiceHostBuilder<TServiceHost, TConfigurator>
        where TParameters :
        IServiceHostBuilderParameters,
        IServiceHostBuilderEventSourceParameters<TEventSourceReplicaTemplate>,
        IServiceHostBuilderEventSourceReplicationParameters<TEventSourceReplicableTemplate, TEventSourceReplicator>,
        IServiceHostBuilderDelegateParameters<TDelegateReplicaTemplate>,
        IServiceHostBuilderDelegateReplicationParameters<TDelegateReplicableTemplate, TDelegateReplicator>,
        IServiceHostBuilderAspNetCoreListenerParameters<TListenerAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerParameters<TListenerRemotingReplicaTemplate>,
        IServiceHostBuilderGenericListenerParameters<TListenerGenericReplicaTemplate>,
        IServiceHostBuilderListenerReplicationParameters<TListenerReplicableTemplate, TListenerReplicator>
        where TConfigurator :
        IServiceHostBuilderConfigurator,
        IServiceHostBuilderEventSourceConfigurator<TEventSourceReplicaTemplate>,
        IServiceHostBuilderEventSourceReplicationConfigurator<TEventSourceReplicableTemplate, TEventSourceReplicator>,
        IServiceHostBuilderDelegateConfigurator<TDelegateReplicaTemplate>,
        IServiceHostBuilderDelegateReplicationConfigurator<TDelegateReplicableTemplate, TDelegateReplicator>,
        IServiceHostBuilderAspNetCoreListenerConfigurator<TListenerAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerConfigurator<TListenerRemotingReplicaTemplate>,
        IServiceHostBuilderGenericListenerConfigurator<TListenerGenericReplicaTemplate>,
        IServiceHostBuilderListenerReplicationConfigurator<TListenerReplicableTemplate, TListenerReplicator>
        where TEventSourceReplicaTemplate :
        TEventSourceReplicableTemplate,
        IServiceHostEventSourceReplicaTemplate<IServiceHostEventSourceReplicaTemplateConfigurator>
        where TEventSourceReplicator : class
        where TDelegateReplicaTemplate :
        TDelegateReplicableTemplate,
        IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>
        where TDelegateReplicator : class
        where TListenerAspNetCoreReplicaTemplate :
        TListenerReplicableTemplate,
        IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        where TListenerRemotingReplicaTemplate :
        TListenerReplicableTemplate,
        IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>
        where TListenerGenericReplicaTemplate :
        TListenerReplicableTemplate,
        IServiceHostGenericListenerReplicaTemplate<IServiceHostGenericListenerReplicaTemplateConfigurator>
        where TListenerReplicator : class
    {
        protected abstract class Parameters
            : IServiceHostBuilderParameters,
              IServiceHostBuilderConfigurator,
              IServiceHostBuilderEventSourceParameters<TEventSourceReplicaTemplate>,
              IServiceHostBuilderEventSourceConfigurator<TEventSourceReplicaTemplate>,
              IServiceHostBuilderEventSourceReplicationParameters<TEventSourceReplicableTemplate, TEventSourceReplicator>,
              IServiceHostBuilderEventSourceReplicationConfigurator<TEventSourceReplicableTemplate, TEventSourceReplicator>,
              IServiceHostBuilderDelegateParameters<TDelegateReplicaTemplate>,
              IServiceHostBuilderDelegateConfigurator<TDelegateReplicaTemplate>,
              IServiceHostBuilderDelegateReplicationParameters<TDelegateReplicableTemplate, TDelegateReplicator>,
              IServiceHostBuilderDelegateReplicationConfigurator<TDelegateReplicableTemplate, TDelegateReplicator>,
              IServiceHostBuilderAspNetCoreListenerParameters<TListenerAspNetCoreReplicaTemplate>,
              IServiceHostBuilderAspNetCoreListenerConfigurator<TListenerAspNetCoreReplicaTemplate>,
              IServiceHostBuilderRemotingListenerParameters<TListenerRemotingReplicaTemplate>,
              IServiceHostBuilderRemotingListenerConfigurator<TListenerRemotingReplicaTemplate>,
              IServiceHostBuilderGenericListenerParameters<TListenerGenericReplicaTemplate>,
              IServiceHostBuilderGenericListenerConfigurator<TListenerGenericReplicaTemplate>,
              IServiceHostBuilderListenerReplicationParameters<TListenerReplicableTemplate, TListenerReplicator>,
              IServiceHostBuilderListenerReplicationConfigurator<TListenerReplicableTemplate, TListenerReplicator>
        {
            public string ServiceTypeName { get; private set; }

            public IServiceHostEventSourceDescriptor EventSourceDescriptor { get; private set; }

            public List<IServiceHostListenerDescriptor> ListenerDescriptors { get; private set; }

            public List<IServiceHostDelegateDescriptor> DelegateDescriptors { get; private set; }

            public Func<TEventSourceReplicaTemplate> EventSourceReplicaTemplateFunc { get; private set; }

            public Func<TEventSourceReplicableTemplate, TEventSourceReplicator> EventSourceReplicatorFunc { get; private set; }

            public Func<TDelegateReplicaTemplate> DelegateReplicaTemplateFunc { get; private set; }

            public Func<TDelegateReplicableTemplate, TDelegateReplicator> DelegateReplicatorFunc { get; private set; }

            public Func<TListenerAspNetCoreReplicaTemplate> AspNetCoreListenerReplicaTemplateFunc { get; private set; }

            public Func<TListenerRemotingReplicaTemplate> RemotingListenerReplicaTemplateFunc { get; private set; }

            public Func<TListenerGenericReplicaTemplate> GenericListenerReplicaTemplateFunc { get; private set; }

            public Func<TListenerReplicableTemplate, TListenerReplicator> ListenerReplicatorFunc { get; private set; }

            public Func<IServiceCollection> DependenciesFunc { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            protected Parameters()
            {
                this.ServiceTypeName = string.Empty;
                this.EventSourceDescriptor = null;
                this.ListenerDescriptors = null;
                this.DelegateDescriptors = null;
                this.EventSourceReplicaTemplateFunc = null;
                this.EventSourceReplicatorFunc = null;
                this.DelegateReplicaTemplateFunc = null;
                this.DelegateReplicatorFunc = null;
                this.AspNetCoreListenerReplicaTemplateFunc = null;
                this.GenericListenerReplicaTemplateFunc = null;
                this.RemotingListenerReplicaTemplateFunc = null;
                this.ListenerReplicatorFunc = null;
                this.DependenciesFunc = DefaultDependenciesFunc;
                this.DependenciesConfigAction = null;
            }

            public void UseServiceType(
                string serviceName)
            {
                this.ServiceTypeName = serviceName
                 ?? throw new ArgumentNullException(nameof(serviceName));
            }

            public void UseEventSourceReplicaTemplate(
                Func<TEventSourceReplicaTemplate> factoryFunc)
            {
                this.EventSourceReplicaTemplateFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseEventSourceReplicator(
                Func<TEventSourceReplicableTemplate, TEventSourceReplicator> factoryFunc)
            {
                this.EventSourceReplicatorFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseDelegateReplicaTemplate(
                Func<TDelegateReplicaTemplate> factoryFunc)
            {
                this.DelegateReplicaTemplateFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseDelegateReplicator(
                Func<TDelegateReplicableTemplate, TDelegateReplicator> factoryFunc)
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

            public void UseGenericListenerReplicaTemplate(
                Func<TListenerGenericReplicaTemplate> factoryFunc)
            {
                this.GenericListenerReplicaTemplateFunc = factoryFunc
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

            public void SetupEventSource(
                Action<TEventSourceReplicaTemplate> setupAction)
            {
                if (setupAction == null)
                {
                    throw new ArgumentNullException(nameof(setupAction));
                }

                this.EventSourceDescriptor = new ServiceHostEventSourceDescriptor(
                    replicaTemplate => setupAction((TEventSourceReplicaTemplate) replicaTemplate));
            }

            public void DefineDelegate(
                Action<TDelegateReplicaTemplate> defineAction)
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
                        replicaTemplate => defineAction((TDelegateReplicaTemplate) replicaTemplate)));
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

            public void DefineGenericListener(
                Action<TListenerGenericReplicaTemplate> defineAction)
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
                        ServiceHostListenerType.Generic,
                        replicaTemplate => defineAction((TListenerGenericReplicaTemplate) replicaTemplate)));
            }

            private static IServiceCollection DefaultDependenciesFunc()
            {
                return new ServiceCollection();
            }
        }

        protected class Compilation
        {
            public TEventSourceReplicator EventSourceReplicator { get; }

            public IReadOnlyList<TDelegateReplicator> DelegateReplicators { get; }

            public IReadOnlyList<TListenerReplicator> ListenerReplicators { get; }

            public Compilation(
                TEventSourceReplicator eventSourceReplicator,
                IReadOnlyList<TDelegateReplicator> delegateReplicators,
                IReadOnlyList<TListenerReplicator> listenerReplicators)
            {
                this.EventSourceReplicator = eventSourceReplicator
                 ?? throw new ArgumentNullException(nameof(eventSourceReplicator));

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
                throw new FactoryProducesNullInstanceException<IServiceCollection>();
            }

            parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

            var dependencies = new DefaultServiceProviderFactory().CreateServiceProvider(dependenciesCollection);

            if (parameters.EventSourceReplicaTemplateFunc == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(parameters.EventSourceReplicaTemplateFunc)} was configured");
            }

            if (parameters.EventSourceReplicatorFunc == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(parameters.EventSourceReplicaTemplateFunc)} was configured");
            }

            var eventSourceReplicaTemplate = parameters.EventSourceReplicaTemplateFunc();
            if (eventSourceReplicaTemplate == null)
            {
                throw new FactoryProducesNullInstanceException<TEventSourceReplicaTemplate>();
            }

            eventSourceReplicaTemplate
               .ConfigureObject(
                    c =>
                    {
                        c.ConfigureDependencies(
                            services =>
                            {
                                var ignore = new HashSet<Type>(services.Select(i => i.ServiceType));
                                services.Proxinate(dependenciesCollection, dependencies, i => !ignore.Contains(i));
                            });
                    });

            parameters.EventSourceDescriptor?.ConfigAction(eventSourceReplicaTemplate);

            var eventSourceReplicator = parameters.EventSourceReplicatorFunc(eventSourceReplicaTemplate);
            if (eventSourceReplicator == null)
            {
                throw new FactoryProducesNullInstanceException<TEventSourceReplicator>();
            }

            var delegateReplicators = parameters.DelegateDescriptors == null
                ? Array.Empty<TDelegateReplicator>()
                : parameters.DelegateDescriptors
                   .Select(
                        descriptor =>
                        {
                            if (parameters.DelegateReplicaTemplateFunc == null)
                            {
                                throw new InvalidOperationException(
                                    $"No {nameof(parameters.DelegateReplicaTemplateFunc)} was configured");
                            }

                            if (parameters.DelegateReplicatorFunc == null)
                            {
                                throw new InvalidOperationException(
                                    $"No {nameof(parameters.DelegateReplicatorFunc)} was configured");
                            }

                            var template = parameters.DelegateReplicaTemplateFunc();
                            if (template == null)
                            {
                                throw new FactoryProducesNullInstanceException<TDelegateReplicaTemplate>();
                            }

                            descriptor.ConfigAction(template);

                            template.ConfigureObject(
                                c =>
                                {
                                    c.ConfigureDependencies(
                                        services =>
                                        {
                                            var ignore = new HashSet<Type>(services.Select(i => i.ServiceType));
                                            services.Proxinate(dependenciesCollection, dependencies, i => !ignore.Contains(i));
                                        });
                                });

                            var replicator = parameters.DelegateReplicatorFunc(template);
                            if (replicator == null)
                            {
                                throw new FactoryProducesNullInstanceException<TDelegateReplicator>();
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

                                        descriptor.ConfigAction(template);

                                        template.ConfigureObject(
                                            c =>
                                            {
                                                c.ConfigureWebHost(
                                                    builder =>
                                                    {
                                                        builder.ConfigureServices(
                                                            services =>
                                                            {
                                                                var ignore = new HashSet<Type>(services.Select(i => i.ServiceType));
                                                                services.Proxinate(dependenciesCollection, dependencies, i => !ignore.Contains(i));
                                                            });
                                                    });
                                            });

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

                                        descriptor.ConfigAction(template);

                                        template.ConfigureObject(
                                            c =>
                                            {
                                                c.ConfigureDependencies(
                                                    services =>
                                                    {
                                                        var ignore = new HashSet<Type>(services.Select(i => i.ServiceType));
                                                        services.Proxinate(dependenciesCollection, dependencies, i => !ignore.Contains(i));
                                                    });
                                            });

                                        replicableTemplate = template;
                                    }
                                    break;
                                case ServiceHostListenerType.Generic:
                                    {
                                        if (parameters.GenericListenerReplicaTemplateFunc == null)
                                        {
                                            throw new InvalidOperationException(
                                                $"No {nameof(parameters.GenericListenerReplicaTemplateFunc)} was configured");
                                        }

                                        var template = parameters.GenericListenerReplicaTemplateFunc();
                                        if (template == null)
                                        {
                                            throw new FactoryProducesNullInstanceException<TListenerGenericReplicaTemplate>();
                                        }

                                        descriptor.ConfigAction(template);

                                        template.ConfigureObject(
                                            c =>
                                            {
                                                c.ConfigureDependencies(
                                                    services =>
                                                    {
                                                        var ignore = new HashSet<Type>(services.Select(i => i.ServiceType));
                                                        services.Proxinate(dependenciesCollection, dependencies, i => !ignore.Contains(i));
                                                    });
                                            });

                                        replicableTemplate = template;
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(descriptor.ListenerType));
                            }

                            if (parameters.ListenerReplicatorFunc == null)
                            {
                                throw new InvalidOperationException(
                                    $"No {nameof(parameters.ListenerReplicatorFunc)} was configured");
                            }

                            var replicator = parameters.ListenerReplicatorFunc(replicableTemplate);
                            if (replicator == null)
                            {
                                throw new FactoryProducesNullInstanceException<TListenerReplicator>();
                            }

                            return replicator;
                        })
                   .ToArray();

            return new Compilation(eventSourceReplicator, delegateReplicators, listenerReplicators);
        }
    }
}