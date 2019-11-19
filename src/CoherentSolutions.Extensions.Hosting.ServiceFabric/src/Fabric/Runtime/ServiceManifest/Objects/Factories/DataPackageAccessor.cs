using System.Fabric;
using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class DataPackageAccessor : PackageAccessor<DataPackage, DataPackageDescription>
    {
        public DataPackageAccessor(
            DataPackage package) 
            : base(package)
        {
        }
    }
}