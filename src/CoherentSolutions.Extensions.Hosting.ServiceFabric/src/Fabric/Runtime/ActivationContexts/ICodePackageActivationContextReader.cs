using System.Fabric;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public interface ICodePackageActivationContextReader
    {
        ICodePackageActivationContext Read(
            ServiceManifestElement manifest);
    }
}