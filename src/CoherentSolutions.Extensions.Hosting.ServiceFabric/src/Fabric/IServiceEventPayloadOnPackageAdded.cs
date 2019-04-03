namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric
{
    public interface IServiceEventPayloadOnPackageAdded<out TPackage>
    {
        TPackage Package { get; }
    }
}