using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostDelegateReplicaTemplate<TService, TParameters, TConfigurator, TDelegate>
        : ConfigurableObject<TConfigurator>, IServiceHostDelegateReplicaTemplate<TConfigurator>
        where TService : IService
        where TParameters : IServiceHostDelegateReplicaTemplateParameters
        where TConfigurator : IServiceHostDelegateReplicaTemplateConfigurator
    {
        protected abstract class DelegateParameters
            : IServiceHostDelegateReplicaTemplateParameters,
              IServiceHostDelegateReplicaTemplateConfigurator
        {
            public Delegate Delegate { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            protected DelegateParameters()
            {
                this.Delegate = null;
                this.DependenciesConfigAction = null;
            }

            public void UseDelegate(
                Delegate @delegate)
            {
                this.Delegate = @delegate
                 ?? throw new ArgumentNullException(nameof(@delegate));
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
        }

        public abstract TDelegate Activate(
            TService service);

        protected Func<IServiceHostDelegate> CreateFunc(
            TService service,
            TParameters parameters)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var serviceContext = service.GetContext();
            var servicePartition = service.GetPartition();
            var serviceEventSource = service.GetEventSource();

            var services = new ServiceCollection();

            DependencyRegistrant.Register(services, serviceContext);
            DependencyRegistrant.Register(services, servicePartition);
            DependencyRegistrant.Register(services, serviceEventSource);

            parameters.DependenciesConfigAction?.Invoke(services);

            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

            return () => new ServiceHostDelegate(parameters.Delegate, provider);
        }
    }
}