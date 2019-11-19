namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public interface IPackageFactory<in TElement, out TPackage>
    {
        TPackage Create(
            TElement element);
    }
}