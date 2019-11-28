using System.Fabric;
using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
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