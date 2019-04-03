namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceEventPayloadOnPackageModified<out TPackage>
    {
        TPackage OldPackage { get; }

        TPackage NewPackage { get; }
    }
}