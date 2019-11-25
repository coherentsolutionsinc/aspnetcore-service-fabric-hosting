using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class ConfigurationSectionAccessor : Accessor<ConfigurationSection>
    {
        private static readonly Lazy<PropertyInfo> name;

        static ConfigurationSectionAccessor()
        {
            name = typeof(ConfigurationSection).QueryProperty(nameof(ConfigurationSection.Name));
        }

        public string Name
        {
            get => this.Instance.Name;
            set => name.Value.SetValue(this.Instance, value);
        }

        public ConfigurationSectionAccessor(
            ConfigurationSection section)
            : base(section)
        {
        }
    }
}