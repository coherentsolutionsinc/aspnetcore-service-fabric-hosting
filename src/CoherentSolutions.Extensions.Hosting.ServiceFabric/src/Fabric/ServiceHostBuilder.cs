using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Proxynator.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Exceptions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;

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
        : ValidateableConfigurableObject<TParameters, TConfigurator>,
          IServiceHostBuilder<TServiceHost, TConfigurator>
        where TParameters :
        class,
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
        class,
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
        class,
        TEventSourceReplicableTemplate,
        IServiceHostEventSourceReplicaTemplate<IServiceHostEventSourceReplicaTemplateConfigurator>
        where TEventSourceReplicator : class
        where TDelegateReplicaTemplate :
        class,
        TDelegateReplicableTemplate,
        IServiceHostDelegateReplicaTemplate<IServiceHostDelegateReplicaTemplateConfigurator>
        where TDelegateReplicator : class
        where TListenerAspNetCoreReplicaTemplate :
        class,
        TListenerReplicableTemplate,
        IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        where TListenerRemotingReplicaTemplate :
        class,
        TListenerReplicableTemplate,
        IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>
        where TListenerGenericReplicaTemplate :
        class,
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

            public List<IServiceHostListenerDescriptor> ListenerDescriptors { get; private set; }

            public List<IServiceHostDelegateDescriptor> DelegateDescriptors { get; private set; }

            [RequiredConfiguration(nameof(UseEventSourceReplicaTemplate))]
            public Func<TEventSourceReplicaTemplate> EventSourceReplicaTemplateFunc { get; private set; }

            [RequiredConfiguration(nameof(UseEventSourceReplicator))]
            public Func<TEventSourceReplicableTemplate, TEventSourceReplicator> EventSourceReplicatorFunc { get; private set; }

            public Action<TEventSourceReplicaTemplate> EventSourceSetupAction { get; private set; }

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
                this.ListenerDescriptors = null;
                this.DelegateDescriptors = null;

                this.EventSourceReplicaTemplateFunc = null;
                this.EventSourceReplicatorFunc = null;
                this.EventSourceSetupAction = null;

                this.DelegateReplicaTemplateFunc = null;
                this.DelegateReplicatorFunc = null;
                this.AspNetCoreListenerReplicaTemplateFunc = null;
                this.GenericListenerReplicaTemplateFunc = null;
                this.RemotingListenerReplicaTemplateFunc = null;
                this.ListenerReplicatorFunc = null;
                this.DependenciesFunc = null;
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
                this.EventSourceSetupAction = setupAction 
                    ?? throw new ArgumentNullException(nameof(setupAction));
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
                this.EventSourceReplicator = eventSourceReplicator;
                this.DelegateReplicators = delegateReplicators;
                this.ListenerReplicators = listenerReplicators;
            }
        }

        public abstract TServiceHost Build();

        protected Compilation CompileParameters(
            TParameters parameters)
        {
            this.ValidateUpstreamConfiguration(parameters);

            // Initialize service dependencies
            var dependenciesCollection = parameters.DependenciesFunc();
            if (dependenciesCollection is null)
            {
                throw new FactoryProducesNullInstanceException<IServiceCollection>();
            }

            parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

            var dependencies = dependenciesCollection.BuildServiceProvider();

            var eventSourceReplicator = ConfigureEventSourceReplicator(parameters, dependenciesCollection, dependencies);

            TDelegateReplicator[] delegateReplicators = null;

            if (parameters.DelegateDescriptors != null)
            {
                delegateReplicators = parameters.DelegateDescriptors
                   .Select(
                        descriptor =>
                        {
                            if (parameters.DelegateReplicaTemplateFunc is null)
                            {
                                throw new InvalidOperationException(
                                    $"No {nameof(parameters.DelegateReplicaTemplateFunc)} was configured");
                            }

                            if (parameters.DelegateReplicatorFunc is null)
                            {
                                throw new InvalidOperationException(
                                    $"No {nameof(parameters.DelegateReplicatorFunc)} was configured");
                            }

                            var template = parameters.DelegateReplicaTemplateFunc();
                            if (template is null)
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
                                            services.Proxinate(dependenciesCollection, dependencies);
                                        });
                                });

                            var replicator = parameters.DelegateReplicatorFunc(template);
                            if (replicator is null)
                            {
                                throw new FactoryProducesNullInstanceException<TDelegateReplicator>();
                            }

                            return replicator;
                        })
                   .ToArray();
            }

            TListenerReplicator[] listenerReplicators = null;

            if (parameters.ListenerDescriptors != null)
            {
                listenerReplicators = parameters.ListenerDescriptors
                   .Select(
                        descriptor =>
                        {
                            TListenerReplicableTemplate replicableTemplate;
                            switch (descriptor.ListenerType)
                            {
                                case ServiceHostListenerType.AspNetCore:
                                    {
                                        if (parameters.AspNetCoreListenerReplicaTemplateFunc is null)
                                        {
                                            throw new InvalidOperationException(
                                                $"No {nameof(parameters.AspNetCoreListenerReplicaTemplateFunc)} was configured");
                                        }

                                        var template = parameters.AspNetCoreListenerReplicaTemplateFunc();
                                        if (template is null)
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
                                                                services.Proxinate(dependenciesCollection, dependencies);
                                                            });
                                                    });
                                            });

                                        replicableTemplate = template;
                                    }
                                    break;
                                case ServiceHostListenerType.Remoting:
                                    {
                                        if (parameters.RemotingListenerReplicaTemplateFunc is null)
                                        {
                                            throw new InvalidOperationException(
                                                $"No {nameof(parameters.RemotingListenerReplicaTemplateFunc)} was configured");
                                        }

                                        var template = parameters.RemotingListenerReplicaTemplateFunc();
                                        if (template is null)
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
                                                        services.Proxinate(dependenciesCollection, dependencies);
                                                    });
                                            });

                                        replicableTemplate = template;
                                    }
                                    break;
                                case ServiceHostListenerType.Generic:
                                    {
                                        if (parameters.GenericListenerReplicaTemplateFunc is null)
                                        {
                                            throw new InvalidOperationException(
                                                $"No {nameof(parameters.GenericListenerReplicaTemplateFunc)} was configured");
                                        }

                                        var template = parameters.GenericListenerReplicaTemplateFunc();
                                        if (template is null)
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
                                                        services.Proxinate(dependenciesCollection, dependencies);
                                                    });
                                            });

                                        replicableTemplate = template;
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(descriptor.ListenerType));
                            }

                            if (parameters.ListenerReplicatorFunc is null)
                            {
                                throw new InvalidOperationException(
                                    $"No {nameof(parameters.ListenerReplicatorFunc)} was configured");
                            }

                            var replicator = parameters.ListenerReplicatorFunc(replicableTemplate);
                            if (replicator is null)
                            {
                                throw new FactoryProducesNullInstanceException<TListenerReplicator>();
                            }

                            return replicator;
                        })
                   .ToArray();
            }

            return new Compilation(eventSourceReplicator, delegateReplicators, listenerReplicators);
        }

        private static TEventSourceReplicator ConfigureEventSourceReplicator(
            TParameters parameters, 
            IServiceCollection dependenciesCollection, 
            IServiceProvider dependenciesProvider)
        {
            var replicaTemplate = parameters.EventSourceReplicaTemplateFunc();
            if (replicaTemplate is null)
            {
                throw new FactoryProducesNullInstanceException<TEventSourceReplicaTemplate>();
            }

            parameters.EventSourceSetupAction?.Invoke(replicaTemplate);

            replicaTemplate
                .ConfigureObject(
                    config =>
                    {
                        config.ConfigureDependencies(
                            dependencies =>
                            {
                                dependencies.Proxinate(dependenciesCollection, dependenciesProvider);
                            });
                    });

            var replicator = parameters.EventSourceReplicatorFunc(replicaTemplate);
            if (replicator is null)
            {
                throw new FactoryProducesNullInstanceException<TEventSourceReplicator>();
            }

            return replicator;
        }
    }
}