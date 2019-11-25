using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest
{
    public class ConfigurationSectionElement
    {
        [XmlElement(ElementName = "Name")]
        public string Name
        {
            get; 
            set;
        }

        [XmlElement(ElementName = "Parameter")]
        public ICollection<ConfigurationParameterElement> Parameters
        {
            get;
            set;
        }
    }
}