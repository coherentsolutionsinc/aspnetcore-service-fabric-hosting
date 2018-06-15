namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceListenerReplicaTemplateParameters
    {
        bool ListenerOnSecondary { get; }
    }
}