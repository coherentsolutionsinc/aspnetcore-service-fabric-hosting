namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceEventPayloadOnPackageRemoved<out TPackage>
    {
        TPackage Package { get; }
    }
}