namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public interface IServiceManifestProvider
    {
        ServiceManifestElement GetManifest();
    }
}