using System.Fabric;
using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class ConfigPackageAccessor : PackageAccessor<ConfigurationPackage, ConfigurationPackageDescription>
    {
        public ConfigPackageAccessor(
            ConfigurationPackage package)
            : base(package)
        {
        }
    }
}