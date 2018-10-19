using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostEventSourceReplicaTemplate
        : ServiceHostEventSourceReplicaTemplate<
              IStatelessServiceHostEventSourceReplicaTemplateParameters, 
              IStatelessServiceHostEventSourceReplicaTemplateConfigurator,
              StatelessServiceEventSource>,
          IStatelessServiceHostEventSourceReplicaTemplate
    {
        private class StatelessEventSourceParameters
            : EventSourceParameters,
              IStatelessServiceHostEventSourceReplicaTemplateParameters,
              IStatelessServiceHostEventSourceReplicaTemplateConfigurator
        {
        }

        public override StatelessServiceEventSource Activate(
            ServiceContext serviceContext)
        {
            var parameters = new StatelessEventSourceParameters();

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateEventSourceFunc(parameters);

            return new StatelessServiceEventSource(() => factory(serviceContext));
        }
    }
}