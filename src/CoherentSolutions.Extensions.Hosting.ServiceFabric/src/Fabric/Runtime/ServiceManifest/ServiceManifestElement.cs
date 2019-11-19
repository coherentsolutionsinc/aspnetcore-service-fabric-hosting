using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest
{
    [XmlRoot(ElementName = "ServiceManifest")]
    public class ServiceManifestElement
    {
        [XmlIgnore]
        public string PackageRoot
        {
            get;
            set;
        }

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
        public PackageElementCollection<CodePackageElement> CodePackages
        {
            get;
            set;
        }

        [XmlElement(ElementName = "ConfigPackage")]
        public PackageElementCollection<ConfigPackageElement> ConfigPackages
        {
            get;
            set;
        }

        [XmlElement(ElementName = "DataPackage")]
        public PackageElementCollection<DataPackageElement> DataPackages
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

        public ServiceManifestElement()
        {
            this.CodePackages = new PackageElementCollection<CodePackageElement>(this);
            this.ConfigPackages = new PackageElementCollection<ConfigPackageElement>(this);
            this.DataPackages = new PackageElementCollection<DataPackageElement>(this);
        }
    }
}