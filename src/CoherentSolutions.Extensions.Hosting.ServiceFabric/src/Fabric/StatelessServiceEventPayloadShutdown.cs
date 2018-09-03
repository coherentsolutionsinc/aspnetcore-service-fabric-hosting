namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatelessServiceEventPayloadShutdown : IStatelessServiceEventPayloadShutdown
    {
        public bool IsAborting { get; }

        public StatelessServiceEventPayloadShutdown(
            bool isAborting)
        {
            this.IsAborting = isAborting;
        }
    }
}