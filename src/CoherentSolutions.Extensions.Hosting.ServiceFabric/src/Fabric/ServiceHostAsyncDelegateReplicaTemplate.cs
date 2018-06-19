using System;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Tools;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Tools;

using Microsoft.Extensions.DependencyInjection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public abstract class ServiceHostAsyncDelegateReplicaTemplate<TService, TParameters, TConfigurator, TDelegate>
        : ConfigurableObject<TConfigurator>, IServiceHostAsyncDelegateReplicaTemplate<TConfigurator>
        where TService : IService
        where TParameters : IServiceHostAsyncDelegateReplicaTemplateParameters
        where TConfigurator : IServiceHostAsyncDelegateReplicaTemplateConfigurator
    {
        protected abstract class DelegateParameters
            : IServiceHostAsyncDelegateReplicaTemplateParameters,
              IServiceHostAsyncDelegateReplicaTemplateConfigurator
        {
            public Delegate Delegate { get; private set; }

            public Action<IServiceCollection> DependenciesConfigAction { get; private set; }

            protected DelegateParameters()
            {
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

        protected Func<IServiceHostAsyncDelegate> CreateFunc(
            TService service,
            TParameters parameters)
        {
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

            return () => new ServiceHostAsyncDelegate(parameters.Delegate, provider);
        }
    }
}