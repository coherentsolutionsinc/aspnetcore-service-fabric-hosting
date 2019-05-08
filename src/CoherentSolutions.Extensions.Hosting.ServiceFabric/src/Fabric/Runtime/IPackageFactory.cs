namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public interface IPackageFactory<TElement, TPackage>
    {
        TPackage Create(TElement element);
    }
}
