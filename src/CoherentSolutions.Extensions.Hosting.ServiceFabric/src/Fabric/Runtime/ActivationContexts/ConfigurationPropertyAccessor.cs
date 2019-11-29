using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class ConfigurationPropertyAccessor
    {
        private static readonly Lazy<PropertyInfo> name;

        private static readonly Lazy<PropertyInfo> value;

        private static readonly Lazy<PropertyInfo> mustOverride;

        private static readonly Lazy<PropertyInfo> isEncrypted;

        private static readonly Lazy<PropertyInfo> type;

        public ConfigurationProperty Instance { get; }

        public string Name
        {
            get => this.Instance.Name;
            set => name.Value.SetValue(this.Instance, value);
        }

        public string Value
        {
            get => this.Instance.Value;
            set => ConfigurationPropertyAccessor.value.Value.SetValue(this.Instance, value);
        }

        public bool MustOverride
        {
            get => this.Instance.MustOverride;
            set => mustOverride.Value.SetValue(this.Instance, value);
        }

        public bool IsEncrypted
        {
            get => this.Instance.IsEncrypted;
            set => isEncrypted.Value.SetValue(this.Instance, value);
        }

        public string Type
        {
            get => this.Instance.Type;
            set => type.Value.SetValue(this.Instance, value);
        }

        static ConfigurationPropertyAccessor()
        {
            name = typeof(ConfigurationProperty).QueryProperty(nameof(ConfigurationProperty.Name));
            value = typeof(ConfigurationProperty).QueryProperty(nameof(ConfigurationProperty.Value));
            mustOverride = typeof(ConfigurationProperty).QueryProperty(nameof(ConfigurationProperty.MustOverride));
            isEncrypted = typeof(ConfigurationProperty).QueryProperty(nameof(ConfigurationProperty.IsEncrypted));
            type = typeof(ConfigurationProperty).QueryProperty(nameof(ConfigurationProperty.Type));
        }

        public ConfigurationPropertyAccessor(
            ConfigurationProperty instance)
        {
            this.Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}