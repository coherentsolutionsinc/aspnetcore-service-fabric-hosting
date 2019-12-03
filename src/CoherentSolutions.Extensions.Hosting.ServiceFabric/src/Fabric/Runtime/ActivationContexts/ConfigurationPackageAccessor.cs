using System;
using System.Fabric;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class ConfigurationPackageAccessor : PackageAccessor<ConfigurationPackage, ConfigurationPackageDescription>
    {
        private static readonly Lazy<PropertyInfo> settings;

        public ConfigurationSettings Settings
        {
            get => this.Instance.Settings;
            set => settings.Value.SetValue(this.Instance, value);
        }

        static ConfigurationPackageAccessor()
        {
            settings = typeof(ConfigurationPackage).QueryProperty(nameof(ConfigurationPackage.Settings));
        }

        public ConfigurationPackageAccessor(
            ConfigurationPackage package)
            : base(package)
        {
        }
    }
}