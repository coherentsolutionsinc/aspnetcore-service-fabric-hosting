using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class DataPackageDescriptionAccessor : PackageDescriptionAccessor<DataPackageDescription>
    {
        public DataPackageDescriptionAccessor(
            DataPackageDescription packageDescription)
            : base(packageDescription)
        {
        }
    }
}