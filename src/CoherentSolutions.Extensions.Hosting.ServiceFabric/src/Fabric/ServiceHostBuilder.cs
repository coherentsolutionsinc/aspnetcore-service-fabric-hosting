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
            TReplicableTemplate,
            TAspNetCoreReplicaTemplate,
            TRemotingReplicaTemplate,
            TReplicator>
        : ConfigurableObject<TConfigurator>,
          IServiceHostBuilder<TServiceHost, TConfigurator>
        where TParameters :
        IServiceHostBuilderParameters,
        IServiceHostBuilderAspNetCoreListenerParameters<TAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerParameters<TRemotingReplicaTemplate>,
        IServiceHostBuilderListenerReplicationParameters<TReplicableTemplate, TReplicator>
        where TConfigurator :
        IServiceHostBuilderConfigurator,
        IServiceHostBuilderAspNetCoreListenerConfigurator<TAspNetCoreReplicaTemplate>,
        IServiceHostBuilderRemotingListenerConfigurator<TRemotingReplicaTemplate>,
        IServiceHostBuilderListenerReplicationConfigurator<TReplicableTemplate, TReplicator>
        where TAspNetCoreReplicaTemplate :
        TReplicableTemplate,
        IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
        where TRemotingReplicaTemplate :
        TReplicableTemplate,
        IServiceHostRemotingListenerReplicaTemplate<IServiceHostRemotingListenerReplicaTemplateConfigurator>
    {
        protected abstract class Parameters
            : IServiceHostBuilderParameters,
              IServiceHostBuilderConfigurator,
              IServiceHostBuilderAspNetCoreListenerParameters<TAspNetCoreReplicaTemplate>,
              IServiceHostBuilderAspNetCoreListenerConfigurator<TAspNetCoreReplicaTemplate>,
              IServiceHostBuilderRemotingListenerParameters<TRemotingReplicaTemplate>,
              IServiceHostBuilderRemotingListenerConfigurator<TRemotingReplicaTemplate>,
              IServiceHostBuilderListenerReplicationParameters<TReplicableTemplate, TReplicator>,
              IServiceHostBuilderListenerReplicationConfigurator<TReplicableTemplate, TReplicator>
        {
            public string ServiceTypeName { get; private set; }

            public List<IServiceHostListenerDescriptor> ListenerDescriptors { get; private set; }

            public Func<TAspNetCoreReplicaTemplate> AspNetCoreListenerReplicaTemplateFunc { get; private set; }

            public Func<TRemotingReplicaTemplate> RemotingListenerReplicaTemplateFunc { get; private set; }

            public Func<TReplicableTemplate, TReplicator> ListenerReplicatorFunc { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            protected Parameters()
            {
                this.ServiceTypeName = string.Empty;
                this.ListenerDescriptors = null;
                this.AspNetCoreListenerReplicaTemplateFunc = null;
                this.RemotingListenerReplicaTemplateFunc = null;
                this.ListenerReplicatorFunc = null;
                this.DependenciesConfigAction = null;
            }

            public void UseServiceType(
                string serviceName)
            {
                this.ServiceTypeName = serviceName
                 ?? throw new ArgumentNullException(nameof(serviceName));
            }

            public void UseAspNetCoreListenerReplicaTemplate(
                Func<TAspNetCoreReplicaTemplate> factoryFunc)
            {
                this.AspNetCoreListenerReplicaTemplateFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseRemotingListenerReplicaTemplate(
                Func<TRemotingReplicaTemplate> factoryFunc)
            {
                this.RemotingListenerReplicaTemplateFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseListenerReplicator(
                Func<TReplicableTemplate, TReplicator> factoryFunc)
            {
                this.ListenerReplicatorFunc = factoryFunc
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

            public void DefineAspNetCoreListener(
                Action<TAspNetCoreReplicaTemplate> defineAction)
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
                        replicaTemplate => defineAction((TAspNetCoreReplicaTemplate) replicaTemplate)));
            }

            public void DefineRemotingListener(
                Action<TRemotingReplicaTemplate> defineAction)
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
                        replicaTemplate => defineAction((TRemotingReplicaTemplate) replicaTemplate)));
            }
        }

        protected class Compilation
        {
            public IServiceProvider Dependencies { get; }
            public IReadOnlyList<TReplicator> Replicators { get; }

            public Compilation(
                IServiceProvider dependencies,
                IReadOnlyList<TReplicator> replicators)
            {
                this.Dependencies = dependencies 
                 ?? throw new ArgumentNullException(nameof(dependencies));

                this.Replicators = replicators 
                 ?? throw new ArgumentNullException(nameof(replicators));
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
            var dependenciesCollection = new ServiceCollection();
            
            parameters.DependenciesConfigAction?.Invoke(dependenciesCollection);

            var dependencies = new DefaultServiceProviderFactory().CreateServiceProvider(dependenciesCollection);

            var replicators = parameters.ListenerDescriptors == null
                ? Array.Empty<TReplicator>()
                : parameters.ListenerDescriptors
                   .Select(
                        descriptor =>
                        {
                            TReplicableTemplate replicableTemplate;
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
                                            throw new FactoryProducesNullInstanceException<TAspNetCoreReplicaTemplate>();
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
                                            throw new FactoryProducesNullInstanceException<TRemotingReplicaTemplate>();
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
                                throw new FactoryProducesNullInstanceException<TReplicator>();
                            }

                            return replicator;
                        })
                   .ToArray();

            return new Compilation(dependencies, replicators);
        }
    }
}