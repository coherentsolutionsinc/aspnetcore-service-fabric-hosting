using System.Fabric;
using System.Fabric.Description;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
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