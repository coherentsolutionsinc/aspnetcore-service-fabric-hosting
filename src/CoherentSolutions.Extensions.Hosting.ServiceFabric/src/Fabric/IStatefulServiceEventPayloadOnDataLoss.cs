namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceEventPayloadOnDataLoss
    {
        IStatefulServiceRestoreContext RestoreContext { get; }
    }
}