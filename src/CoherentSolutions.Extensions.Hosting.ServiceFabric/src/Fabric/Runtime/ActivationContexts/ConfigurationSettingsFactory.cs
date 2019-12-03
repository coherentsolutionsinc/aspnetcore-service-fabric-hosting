using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class ConfigurationSettingsFactory
    {
        private static readonly Lazy<ConstructorInfo> ctor;

        static ConfigurationSettingsFactory()
        {
            ctor = typeof(ConfigurationSettings).QueryConstructor(@public: false);
        }

        public ConfigurationSettings Create(
            ConfigurationSettingsElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var instance = (ConfigurationSettings) ctor.Value.Invoke(null);

            var sectionFactory = new ConfigurationSectionFactory();
            foreach (var item in element.Sections)
            {
                instance.Sections.Add(sectionFactory.Create(item));
            }

            return instance;
        }
    }
}