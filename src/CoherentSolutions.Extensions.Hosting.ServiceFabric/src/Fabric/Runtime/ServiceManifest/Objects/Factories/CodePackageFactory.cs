using System.Fabric;
using System.Fabric.Description;
namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class CodePackageFactory : PackageFactory<CodePackageElement, CodePackage, CodePackageDescription>
    {
        protected override PackageAccessor<CodePackage, CodePackageDescription> CreatePackage(
            CodePackage package)
        {
            return new CodePackageAccessor(package);
        }

        protected override PackageDescriptionAccessor<CodePackageDescription> CreatePackageDescription(
            CodePackageDescription packageDescription)
        {
            return new CodePackageDescriptionAccessor(packageDescription);
        }
    }
}