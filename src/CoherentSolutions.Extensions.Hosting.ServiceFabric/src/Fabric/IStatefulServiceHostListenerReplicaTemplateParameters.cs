namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IStatefulServiceHostListenerReplicaTemplateParameters
    {
        bool ListenerOnSecondary { get; }
    }
}