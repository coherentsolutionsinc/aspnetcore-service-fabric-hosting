namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceEventPayloadOnShutdown : IStatefulServiceEventPayloadOnShutdown
    {
        public bool IsAborting { get; }

        public StatefulServiceEventPayloadOnShutdown(
            bool isAborting)
        {
            this.IsAborting = isAborting;
        }
    }
}