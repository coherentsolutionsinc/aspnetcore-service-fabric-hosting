using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
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