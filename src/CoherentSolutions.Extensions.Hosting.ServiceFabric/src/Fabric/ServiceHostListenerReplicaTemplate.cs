using System;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Validation.DataAnnotations;

using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostListenerReplicaTemplate<TService, TParameters, TConfigurator, TListener>
        : ServiceHostReplicaTemplate<TService, TListener, TParameters, TConfigurator>, IServiceHostListenerReplicaTemplate<TConfigurator>
        where TService : IService
        where TParameters : IServiceHostListenerReplicaTemplateParameters
        where TConfigurator : IServiceHostListenerReplicaTemplateConfigurator
    {
        protected abstract class ListenerParameters : ReplicaTemplateParameters,
            IServiceHostListenerReplicaTemplateParameters,
            IServiceHostListenerReplicaTemplateConfigurator
        {
            [RequiredConfiguration(nameof(UseEndpoint))]
            public string EndpointName
            {
                get; private set;
            }

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

        protected abstract Func<TService, ICommunicationListener> CreateFactory(
            TParameters parameters);
    }
}