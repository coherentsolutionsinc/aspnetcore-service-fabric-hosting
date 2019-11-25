using System;
using System.Fabric.Description;
using System.Reflection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class ConfigurationPackageDescriptionAccessor : PackageDescriptionAccessor<ConfigurationPackageDescription>
    {
        private static readonly Lazy<PropertyInfo> settings;

        static ConfigurationPackageDescriptionAccessor()
        {
            settings = typeof(ConfigurationPackageDescription).QueryProperty(nameof(ConfigurationPackageDescription.Settings));
        }

        public ConfigurationSettings Settings
        {
            get => this.Instance.Settings;
            set => settings.Value.SetValue(this.Instance, value);
        }

        public ConfigurationPackageDescriptionAccessor(
            ConfigurationPackageDescription packageDescription)
            : base(packageDescription)
        {
        }
    }
}