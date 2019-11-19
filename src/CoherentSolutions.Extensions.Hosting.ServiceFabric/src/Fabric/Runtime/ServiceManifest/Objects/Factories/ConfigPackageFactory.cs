using System;
using System.Fabric;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class ConfigPackageFactory : PackageFactory<ConfigPackageElement, ConfigurationPackage, ConfigurationPackageDescription>
    {
        protected override PackageAccessor<ConfigurationPackage, ConfigurationPackageDescription> CreatePackage(
            ConfigurationPackage package)
        {
            return new ConfigPackageAccessor(package);
        }

        protected override PackageDescriptionAccessor<ConfigurationPackageDescription> CreatePackageDescription(
            ConfigurationPackageDescription packageDescription)
        {
            return new ConfigPackageDescriptionAccessor(packageDescription);
        }
    }
}