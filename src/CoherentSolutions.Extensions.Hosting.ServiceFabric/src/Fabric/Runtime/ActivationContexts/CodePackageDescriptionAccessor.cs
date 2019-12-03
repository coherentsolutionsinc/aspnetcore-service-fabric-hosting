using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
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