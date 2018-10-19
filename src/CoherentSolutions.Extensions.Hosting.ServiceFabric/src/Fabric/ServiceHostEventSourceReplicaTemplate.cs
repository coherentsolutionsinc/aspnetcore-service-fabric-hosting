using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostEventSourceReplicaTemplate<TParameters, TConfigurator>
        : ConfigurableObject<TConfigurator>, IServiceHostEventSourceReplicaTemplate<TConfigurator>
        where TParameters : IServiceHostEventSourceReplicaTemplateParameters
        where TConfigurator : IServiceHostEventSourceReplicaTemplateConfigurator
    {
        protected abstract class EventSourceParameters
            : IServiceHostEventSourceReplicaTemplateParameters,
              IServiceHostEventSourceReplicaTemplateConfigurator
        {
            public Func<IServiceProvider, IServiceEventSource> ImplementationFunc { get; private set; }

            protected EventSourceParameters()
            {
                this.ImplementationFunc = DefaultImplementationFactory;
            }

            public void UseImplementation(
                Func<IServiceProvider, IServiceEventSource> factoryFunc)
            {
                this.ImplementationFunc = factoryFunc
                 ?? throw new ArgumentNullException(nameof(factoryFunc));
            }

            private static IServiceEventSource DefaultImplementationFactory(
                IServiceProvider serviceProvider)
            {
                return ActivatorUtilities.CreateInstance<ServiceHostEventSource>(serviceProvider);
            }
        }

        public abstract IServiceEventSource Activate(
            ServiceContext serviceContext);

        protected Func<ServiceContext, IServiceEventSource> CreateDelegateInvokerFunc(
            TParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var build = new Func<ServiceContext, IServiceEventSource>(
                serviceContext =>
                {
                    var serviceCollection = new ServiceCollection();

                    serviceCollection.Add(serviceContext);
                    
                    return null;
                });

            return build;
        }
    }
}