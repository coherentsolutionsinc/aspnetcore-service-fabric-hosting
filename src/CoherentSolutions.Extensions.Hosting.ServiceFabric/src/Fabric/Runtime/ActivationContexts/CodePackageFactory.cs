using System.Fabric;
using System.Fabric.Description;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class CodePackageFactory : PackageFactory<CodePackageElement, CodePackage, CodePackageDescription>
    {
    }
}