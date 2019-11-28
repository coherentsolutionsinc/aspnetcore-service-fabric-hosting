using System.Fabric;
using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
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