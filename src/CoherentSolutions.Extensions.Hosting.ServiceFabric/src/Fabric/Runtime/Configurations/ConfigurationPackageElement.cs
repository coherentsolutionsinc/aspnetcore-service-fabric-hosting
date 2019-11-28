using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class ConfigurationPackageElement : PackageElement
    {
        [XmlIgnore]
        public ConfigurationSettingsElement Settings
        {
            get;
            set;
        }
    }
}