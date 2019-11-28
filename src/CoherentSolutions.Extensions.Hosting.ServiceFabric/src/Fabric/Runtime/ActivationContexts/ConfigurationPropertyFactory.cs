using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class ConfigurationPropertyFactory
    {
        private static readonly Lazy<ConstructorInfo> ctor;

        static ConfigurationPropertyFactory()
        {
            ctor = typeof(ConfigurationProperty).QueryConstructor(@public: false);
        }

        public ConfigurationProperty Create(
            ConfigurationParameterElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return new ConfigurationPropertyAccessor(
                    (ConfigurationProperty)ctor.Value.Invoke(null))
                {
                    Name = element.Name,
                    Value = element.Value,
                    MustOverride = element.MustOverride,
                    IsEncrypted = element.IsEncrypted,
                    Type = element.Type
                }
               .Instance;
        }
    }
}