namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceHostEventSourceReplicaTemplate
        : ServiceHostEventSourceReplicaTemplate<
              IStatelessServiceInformation,
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
            IStatelessServiceInformation serviceInformation)
        {
            var parameters = new StatelessEventSourceParameters();

            this.UpstreamConfiguration(parameters);

            var factory = this.CreateFactory(parameters);

            return new StatelessServiceEventSource(() => factory(serviceInformation));
        }
    }
}