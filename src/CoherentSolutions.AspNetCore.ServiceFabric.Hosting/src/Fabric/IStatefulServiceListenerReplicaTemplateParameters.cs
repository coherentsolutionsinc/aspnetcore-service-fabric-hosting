namespace CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Fabric
{
    public interface IStatefulServiceListenerReplicaTemplateParameters
    {
        bool ListenerOnSecondary { get; }
    }
}