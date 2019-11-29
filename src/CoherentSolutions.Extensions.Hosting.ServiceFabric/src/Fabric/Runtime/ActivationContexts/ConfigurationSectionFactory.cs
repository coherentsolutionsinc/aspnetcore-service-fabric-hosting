using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class ConfigurationSectionFactory
    {
        private static readonly Lazy<ConstructorInfo> ctor;

        static ConfigurationSectionFactory()
        {
            ctor = typeof(ConfigurationSection).QueryConstructor(@public: false);
        }

        public ConfigurationSection Create(
            ConfigurationSectionElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var instance = new ConfigurationSectionAccessor(
                    (ConfigurationSection) ctor.Value.Invoke(null))
                {
                    Name = element.Name
                }
               .Instance;

            var propertyFactory = new ConfigurationPropertyFactory();
            foreach (var item in element.Parameters)
            {
                instance.Parameters.Add(propertyFactory.Create(item));
            }

            return instance;
        }
    }
}