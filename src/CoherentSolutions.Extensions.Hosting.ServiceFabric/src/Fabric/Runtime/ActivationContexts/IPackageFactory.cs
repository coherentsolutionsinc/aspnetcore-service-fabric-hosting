namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public interface IPackageFactory<in TElement, out TPackage>
    {
        TPackage Create(
            TElement element);
    }
}