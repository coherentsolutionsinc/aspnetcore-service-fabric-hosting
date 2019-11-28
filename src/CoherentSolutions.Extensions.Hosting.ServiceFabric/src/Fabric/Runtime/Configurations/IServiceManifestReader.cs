namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public interface IServiceManifestReader
    {
        ServiceManifestElement Read(
            IServicePackage package);
    }
}