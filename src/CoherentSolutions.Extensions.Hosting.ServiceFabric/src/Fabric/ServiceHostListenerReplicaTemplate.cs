using System;
using System.Fabric;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener>
        : ValidateableConfigurableObject<TParameters, TConfigurator>, IServiceHostListenerReplicaTemplate<TConfigurator>
        where TService : IService
        where TParameters : IServiceHostListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostListenerReplicaTemplateConfigurator
    {
        protected abstract class ListenerParameters
            : IServiceHostListenerReplicaTemplateParameters,
              IServiceHostListenerReplicaTemplateConfigurator
        {
            [RequiredConfiguration(nameof(UseEndpoint))]
            public string EndpointName { get; private set; }

            protected ListenerParameters()
            {
                this.EndpointName = null;
            }

            public void UseEndpoint(
                string endpointName)
            {
                this.EndpointName = endpointName
                 ?? throw new ArgumentNullException(nameof(endpointName));
            }
        }

        public abstract TListener Activate(
            TService service);

        protected abstract Func<TService, ICommunicationListener> CreateFactory(
            TParameters parameters);
    }
}