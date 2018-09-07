namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceEventPayloadOnShutdown : IStatelessServiceEventPayloadOnShutdown
    {
        public bool IsAborting { get; }

        public StatelessServiceEventPayloadOnShutdown(
            bool isAborting)
        {
            this.IsAborting = isAborting;
        }
    }
}