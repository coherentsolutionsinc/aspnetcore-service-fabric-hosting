namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public interface IServicePackageProvider
    {
        IServicePackage GetPackage();
    }
}