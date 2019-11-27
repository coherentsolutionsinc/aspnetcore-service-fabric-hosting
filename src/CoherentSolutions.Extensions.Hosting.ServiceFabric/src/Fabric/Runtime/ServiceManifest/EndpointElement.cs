using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest
{
    public class EndpointElement
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "Protocol")]
        public string Protocol
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "Port")]
        public string Port
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

        [XmlAttribute(AttributeName = "CodePackageRef")]
        public string CodePackageRef
        {
            get;
            set;
        }
    }
}