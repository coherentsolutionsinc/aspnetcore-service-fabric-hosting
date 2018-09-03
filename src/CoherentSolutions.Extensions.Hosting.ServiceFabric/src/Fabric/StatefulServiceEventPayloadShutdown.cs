namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public class StatefulServiceEventPayloadShutdown : IStatefulServiceEventPayloadShutdown
    {
        public bool IsAborting { get; }

        public StatefulServiceEventPayloadShutdown(
            bool isAborting)
        {
            this.IsAborting = isAborting;
        }
    }
}