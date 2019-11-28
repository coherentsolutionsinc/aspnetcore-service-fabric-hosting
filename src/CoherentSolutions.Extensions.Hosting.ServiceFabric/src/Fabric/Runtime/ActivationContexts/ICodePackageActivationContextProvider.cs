using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public interface ICodePackageActivationContextProvider
    {
        ICodePackageActivationContext GetActivationContext();
    }
}