using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest
{
    [XmlRoot(ElementName = "ServiceManifest")]
    public class ServiceManifestElement
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute(AttributeName = "Version")]
        public string Version
        {
            get;
            set;
        }

        [XmlArray(ElementName = "ServiceTypes")]
        [XmlArrayItem(ElementName = "StatelessServiceType", Type = typeof(StatelessServiceTypeElement))]
        public ServiceTypeElement[] ServiceTypes
        {
            get;
            set;
        }

        [XmlElement(ElementName = "CodePackage")]
        public CodePackageElement[] CodePackages
        {
            get;
            set;
        }

        [XmlElement(ElementName = "ConfigPackage")]
        public ConfigPackageElement[] ConfigPackages
        {
            get;
            set;
        }

        [XmlElement(ElementName = "DataPackage")]
        public DataPackageElement[] DataPackages
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Resources")]
        public ResourcesElement Resources
        {
            get;
            set;
        }
    }
}