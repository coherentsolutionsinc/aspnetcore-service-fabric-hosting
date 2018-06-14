using System;
using System.Fabric;

using CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tools;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
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

            public Func<IServiceHostListenerLoggerOptions> LoggerOptionsFunc { get; private set; }

            protected ListenerParameters()
            {
                this.EndpointName = string.Empty;
                this.LoggerOptionsFunc = DefaultLoggerOptionsFunc;
            }

            public void UseEndpointName(
                string endpointName)
            {
                this.EndpointName = endpointName
                 ?? throw new ArgumentNullException(nameof(endpointName));
            }

            public void UseLoggerOptions(
                Func<IServiceHostListenerLoggerOptions> factoryFunc)
            {
                this.LoggerOptionsFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            private static IServiceHostListenerLoggerOptions DefaultLoggerOptionsFunc()
            {
                return new ServiceHostListenerLoggerOptions();
            }
        }

        public abstract TListener Activate(
            TService service);

        protected abstract Func<ServiceContext, ICommunicationListener> CreateCommunicationListenerFunc(
            TService service,
            TParameters parameters);
    }
}