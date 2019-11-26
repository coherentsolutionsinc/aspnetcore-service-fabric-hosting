using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest
{
    [XmlRoot(ElementName = "Settings")]
    public class ConfigurationSettingsElement
    {
        [XmlElement(ElementName = "Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Section")]
        public List<ConfigurationSectionElement> Sections
        {
            get;
            set;
        }
    }
}