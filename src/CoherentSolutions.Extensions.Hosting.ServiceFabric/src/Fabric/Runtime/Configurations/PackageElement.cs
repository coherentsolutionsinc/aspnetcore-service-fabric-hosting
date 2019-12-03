using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public abstract class PackageElement
    {
        [XmlIgnore]
        public ServiceManifestElement Manifest { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
    }
}