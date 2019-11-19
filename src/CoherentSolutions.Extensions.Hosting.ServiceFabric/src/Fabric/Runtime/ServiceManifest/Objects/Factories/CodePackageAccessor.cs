using System.Fabric;
using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class CodePackageAccessor : PackageAccessor<CodePackage, CodePackageDescription>
    {
        public CodePackageAccessor(
            CodePackage package)
            : base(package)
        {
        }
    }
}