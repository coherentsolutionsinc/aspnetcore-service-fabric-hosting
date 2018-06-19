using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostAsyncDelegateReplicaTemplate
        : ServiceHostAsyncDelegateReplicaTemplate<
              IStatefulService,
              IStatefulServiceHostAsyncDelegateReplicaTemplateParameters,
              IStatefulServiceHostAsyncDelegateReplicaTemplateConfigurator,
              IServiceHostAsyncDelegate>,
          IStatefulServiceHostAsyncDelegateReplicaTemplate
    {
        private class StatefulAsyncDelegateParameters
            : DelegateParameters,
              IStatefulServiceHostAsyncDelegateReplicaTemplateParameters,
              IStatefulServiceHostAsyncDelegateReplicaTemplateConfigurator
        {
        }

        public override IServiceHostAsyncDelegate Activate(
            IStatefulService service)
        {
            var parameters = new StatefulAsyncDelegateParameters();

            parameters.ConfigureDependencies(
                dependencies =>
                {
                    dependencies.Add(new ServiceDescriptor(typeof(IReliableStateManager), service.GetReliableStateManager()));
                });

            this.UpstreamConfiguration(parameters);

            var func = this.CreateFunc(service, parameters);

            return func();
        }
    }
}