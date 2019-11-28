using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class ConfigurationSectionAccessor
    {
        private static readonly Lazy<PropertyInfo> name;

        public ConfigurationSection Instance
        {
            get;
        }

        public string Name
        {
            get => this.Instance.Name;
            set => name.Value.SetValue(this.Instance, value);
        }

        static ConfigurationSectionAccessor()
        {
            name = typeof(ConfigurationSection).QueryProperty(nameof(ConfigurationSection.Name));
        }

        public ConfigurationSectionAccessor(
            ConfigurationSection instance)
        {
            this.Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}