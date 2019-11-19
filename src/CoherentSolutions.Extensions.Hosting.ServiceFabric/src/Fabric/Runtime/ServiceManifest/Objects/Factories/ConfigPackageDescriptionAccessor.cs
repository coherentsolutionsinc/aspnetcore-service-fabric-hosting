using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class ConfigPackageDescriptionAccessor : PackageDescriptionAccessor<ConfigurationPackageDescription>
    {
        public ConfigPackageDescriptionAccessor(
            ConfigurationPackageDescription packageDescription)
            : base(packageDescription)
        {
        }
    }
}