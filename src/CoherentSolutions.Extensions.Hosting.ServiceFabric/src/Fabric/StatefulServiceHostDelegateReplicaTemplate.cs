using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostDelegateReplicaTemplate
        : ServiceHostDelegateReplicaTemplate<
              IStatefulService,
              IStatefulServiceHostDelegateReplicaTemplateParameters,
              IStatefulServiceHostDelegateReplicaTemplateConfigurator,
              IServiceHostDelegate>,
          IStatefulServiceHostDelegateReplicaTemplate
    {
        private class StatefulAsyncDelegateParameters
            : DelegateParameters,
              IStatefulServiceHostDelegateReplicaTemplateParameters,
              IStatefulServiceHostDelegateReplicaTemplateConfigurator
        {
        }

        public override IServiceHostDelegate Activate(
            IStatefulService service)
        {
            var parameters = new StatefulAsyncDelegateParameters();

            parameters.ConfigureDependencies(
                dependencies =>
                {
                    dependencies.Add(new ServiceDescriptor(typeof(IReliableStateManager), service.GetReliableStateManager()));
                });

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateFunc(service, parameters);

            return factory();
        }
    }
}