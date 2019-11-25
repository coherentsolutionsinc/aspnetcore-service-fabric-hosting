using System.Fabric;
using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{

    public class ConfigurationPackageFactory : PackageFactory<ConfigurationPackageElement, ConfigurationPackage, ConfigurationPackageDescription>
    {
        protected override void InitializePackageDescription(
            ConfigurationPackageDescription packageDescription,
            ConfigurationPackageElement element)
        {
            _ = new ConfigurationPackageDescriptionAccessor(
                packageDescription)
            {
                Settings = element.Settings is object 
                    ? new ConfigurationSettingsFactory().Create(element.Settings)
                    : null
            };
        }

        protected override void InitializePackage(
            ConfigurationPackage package,
            ConfigurationPackageElement element)
        {
            _ = new ConfigurationPackageAccessor(
                package)
            {
                Settings = package.Description.Settings
            };
        }
    }
}