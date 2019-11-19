using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class CodePackageDescriptionAccessor : PackageDescriptionAccessor<CodePackageDescription>
    {
        public CodePackageDescriptionAccessor(
            CodePackageDescription packageDescription) 
            : base(packageDescription)
        {
        }
    }
}