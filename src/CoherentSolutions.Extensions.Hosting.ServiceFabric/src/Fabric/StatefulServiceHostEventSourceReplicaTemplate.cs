using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceHostEventSourceReplicaTemplate
        : ServiceHostEventSourceReplicaTemplate<
              IStatefulServiceInformation,
              IStatefulServiceHostEventSourceReplicaTemplateParameters,
              IStatefulServiceHostEventSourceReplicaTemplateConfigurator,
              StatefulServiceEventSource>,
          IStatefulServiceHostEventSourceReplicaTemplate
    {
        private class StatefulEventSourceParameters
            : EventSourceParameters,
              IStatefulServiceHostEventSourceReplicaTemplateParameters,
              IStatefulServiceHostEventSourceReplicaTemplateConfigurator
        {
        }

        public override StatefulServiceEventSource Activate(
            IStatefulServiceInformation serviceInformation)
        {
            var parameters = new StatefulEventSourceParameters();

            parameters.ConfigureDependencies(
                dependencies =>
                {
                    dependencies.Add(new ServiceDescriptor(typeof(IReliableStateManager), serviceInformation.GetReliableStateManager()));
                });

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateEventSourceFunc(parameters);

            return new StatefulServiceEventSource(() => factory(serviceInformation));
        }
    }
}