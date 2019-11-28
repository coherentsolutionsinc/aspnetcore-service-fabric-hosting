using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class ConfigurationParameterElement
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "Value")]
        public string Value
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "MustOverride")]
        public bool MustOverride
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "IsEncrypted")]
        public bool IsEncrypted
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get;
            set;
        }
    }
}