using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    // Disabling "obsolete" warning because these are runtime stubs
    //
    #pragma warning disable CS0618

    public class ConfigurationPackageDescriptionAccessor : PackageDescriptionAccessor<ConfigurationPackageDescription>
    {
        private static readonly Lazy<PropertyInfo> settings;

        public ConfigurationSettings Settings
        {
            get => this.Instance.Settings;
            set => settings.Value.SetValue(this.Instance, value);
        }

        static ConfigurationPackageDescriptionAccessor()
        {
            settings = typeof(ConfigurationPackageDescription).QueryProperty(nameof(ConfigurationPackageDescription.Settings));
        }

        public ConfigurationPackageDescriptionAccessor(
            ConfigurationPackageDescription packageDescription)
            : base(packageDescription)
        {
        }
    }

    #pragma warning restore CS0618
}