using System;
using System.Collections.Generic;
using System.Linq;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Common.Exceptions;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;
using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Web;

using Microsoft.AspNetCore.Hosting;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public abstract class ServiceHostBuilder<
            TServiceHost,
            TParameters,
            TConfigurator,
            TReplicableTemplate,
            TAspNetCoreReplicaTemplate,
            TReplicator>
        : ConfigurableObject<TConfigurator>,
          IServiceHostBuilder<TServiceHost, TConfigurator>
        where TParameters :
        IServiceHostBuilderParameters,
        IServiceHostBuilderAspNetCoreListenerParameters<TAspNetCoreReplicaTemplate>,
        IServiceHostBuilderListenerReplicationParameters<TReplicableTemplate, TReplicator>
        where TConfigurator :
        IServiceHostBuilderConfigurator,
        IServiceHostBuilderAspNetCoreListenerConfigurator<TAspNetCoreReplicaTemplate>,
        IServiceHostBuilderListenerReplicationConfigurator<TReplicableTemplate, TReplicator>
        where TAspNetCoreReplicaTemplate :
        TReplicableTemplate,
        IServiceHostAspNetCoreListenerReplicaTemplate<IServiceHostAspNetCoreListenerReplicaTemplateConfigurator>
    {
        protected abstract class Parameters
            : IServiceHostBuilderParameters,
              IServiceHostBuilderConfigurator,
              IServiceHostBuilderAspNetCoreListenerParameters<TAspNetCoreReplicaTemplate>,
              IServiceHostBuilderAspNetCoreListenerConfigurator<TAspNetCoreReplicaTemplate>,
              IServiceHostBuilderListenerReplicationParameters<TReplicableTemplate, TReplicator>,
              IServiceHostBuilderListenerReplicationConfigurator<TReplicableTemplate, TReplicator>
        {
            public string ServiceName { get; private set; }

            public List<IServiceHostListenerDescriptor> ListenerDescriptors { get; private set; }

            public Func<TAspNetCoreReplicaTemplate> AspNetCoreListenerReplicaTemplateFunc { get; private set; }

            public Func<TReplicableTemplate, TReplicator> ListenerReplicatorFunc { get; private set; }

            public Func<IWebHostBuilderExtensionsImpl> WebHostBuilderExtensionsImplFunc { get; private set; }

            public Func<IWebHostExtensionsImpl> WebHostExtensionsImplFunc { get; private set; }

            public Func<IWebHostBuilder> WebHostBuilderFunc { get; private set; }

            public Action<IWebHostBuilder> WebHostConfigAction { get; private set; }

            protected Parameters()
            {
                this.ServiceName = string.Empty;
                this.ListenerDescriptors = null;
                this.AspNetCoreListenerReplicaTemplateFunc = null;
                this.ListenerReplicatorFunc = null;
                this.WebHostBuilderExtensionsImplFunc = DefaultWebHostBuilderExtensionsImplFunc;
                this.WebHostExtensionsImplFunc = DefaultWebHostExtensionsImplFunc;
                this.WebHostBuilderFunc = DefaultWebHostBuilderFunc;
                this.WebHostConfigAction = null;
            }

            public void UseServiceName(
                string serviceName)
            {
                this.ServiceName = serviceName
                 ?? throw new ArgumentNullException(nameof(serviceName));
            }

            public void UseAspNetCoreListenerReplicaTemplate(
                Func<TAspNetCoreReplicaTemplate> factoryFunc)
            {
                this.AspNetCoreListenerReplicaTemplateFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseListenerReplicator(
                Func<TReplicableTemplate, TReplicator> factoryFunc)
            {
                this.ListenerReplicatorFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseWebHostBuilderExtensionsImpl(
                Func<IWebHostBuilderExtensionsImpl> factoryFunc)
            {
                this.WebHostBuilderExtensionsImplFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseWebHostExtensionsImpl(
                Func<IWebHostExtensionsImpl> factoryFunc)
            {
                this.WebHostExtensionsImplFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void UseWebHostBuilder(
                Func<IWebHostBuilder> factoryFunc)
            {
                this.WebHostBuilderFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            public void ConfigureWebHost(
                Action<IWebHostBuilder> configAction)
            {
                if (configAction == null)
                {
                    throw new ArgumentNullException(nameof(configAction));
                }

                this.WebHostConfigAction = this.WebHostConfigAction.Chain(configAction);
            }

            public void DefineAspNetCoreListener(
                Action<TAspNetCoreReplicaTemplate> declareAction)
            {
                if (declareAction == null)
                {
                    throw new ArgumentNullException(nameof(declareAction));
                }

                if (this.ListenerDescriptors == null)
                {
                    this.ListenerDescriptors = new List<IServiceHostListenerDescriptor>();
                }

                this.ListenerDescriptors.Add(
                    new ServiceHostListenerDescriptor(
                        ServiceHostListenerType.AspNetCore,
                        replicaTemplate => declareAction((TAspNetCoreReplicaTemplate) replicaTemplate)));
            }

            private static IWebHostBuilderExtensionsImpl DefaultWebHostBuilderExtensionsImplFunc()
            {
                return new WebHostBuilderExtensionsImpl();
            }

            private static IWebHostExtensionsImpl DefaultWebHostExtensionsImplFunc()
            {
                return new WebHostExtensionsImpl();
            }

            private static IWebHostBuilder DefaultWebHostBuilderFunc()
            {
                return new WebHostBuilder();
            }
        }

        public abstract TServiceHost Build();

        protected IEnumerable<TReplicator> BuildReplicators(
            TParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.AspNetCoreListenerReplicaTemplateFunc == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(parameters.AspNetCoreListenerReplicaTemplateFunc)} was configured");
            }

            if (parameters.ListenerReplicatorFunc == null)
            {
                throw new InvalidOperationException(
                    $"No {nameof(parameters.ListenerReplicatorFunc)} was configured");
            }

            var replicators = parameters.ListenerDescriptors == null
                ? Enumerable.Empty<TReplicator>()
                : parameters.ListenerDescriptors
                   .Select(
                        descriptor =>
                        {
                            switch (descriptor.ListenerType)
                            {
                                case ServiceHostListenerType.AspNetCore:
                                    {
                                        var replicaTemplate = parameters.AspNetCoreListenerReplicaTemplateFunc();
                                        if (replicaTemplate == null)
                                        {
                                            throw new FactoryProducesNullInstanceException<TAspNetCoreReplicaTemplate>();
                                        }

                                        replicaTemplate.ConfigureObject(
                                            c =>
                                            {
                                                c.UseWebHostBuilderExtensionsImpl(parameters.WebHostBuilderExtensionsImplFunc);
                                                c.UseWebHostBuilder(parameters.WebHostBuilderFunc);
                                            });

                                        descriptor.ConfigAction(replicaTemplate);

                                        var replicator = parameters.ListenerReplicatorFunc(replicaTemplate);
                                        if (replicator == null)
                                        {
                                            throw new FactoryProducesNullInstanceException<TReplicator>();
                                        }

                                        return replicator;
                                    }
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(descriptor.ListenerType));
                            }
                        })
                   .ToArray();

            return replicators;
        }
    }
}