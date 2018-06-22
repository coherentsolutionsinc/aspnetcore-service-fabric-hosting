using System;
using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener>
        : ConfigurableObject<TConfigurator>, IServiceHostListenerReplicaTemplate<TConfigurator>
        where TService : IService
        where TParameters : IServiceHostListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostListenerReplicaTemplateConfigurator
    {
        protected abstract class ListenerParameters
            : IServiceHostListenerReplicaTemplateParameters,
              IServiceHostListenerReplicaTemplateConfigurator
        {
            public string EndpointName { get; private set; }

            public Func<IServiceHostLoggerOptions> LoggerOptionsFunc { get; private set; }

            public Func<IServiceCollection> DependenciesFunc { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            protected ListenerParameters()
            {
                this.EndpointName = string.Empty;
                this.LoggerOptionsFunc = DefaultLoggerOptionsFunc;
                this.DependenciesFunc = DefaultDependenciesFunc;
                this.DependenciesConfigAction = null;
            }

            public void UseEndpointName(
                string endpointName)
            {
                this.EndpointName = endpointName
                 ?? throw new ArgumentNullException(nameof(endpointName));
            }

            public void UseLoggerOptions(
                Func<IServiceHostLoggerOptions> factoryFunc)
            {
                this.LoggerOptionsFunc = factoryFunc
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

            private static IServiceHostLoggerOptions DefaultLoggerOptionsFunc()
            {
                return ServiceHostLoggerOptions.Disabled;
            }

            private static IServiceCollection DefaultDependenciesFunc()
            {
                return new ServiceCollection();
            }
        }

        public abstract TListener Activate(
            TService service);

        protected abstract Func<ServiceContext, ICommunicationListener> CreateCommunicationListenerFunc(
            TService service,
            TParameters parameters);
    }
}