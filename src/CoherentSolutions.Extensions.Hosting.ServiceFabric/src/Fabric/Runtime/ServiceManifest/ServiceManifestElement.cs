using System.Collections.Generic;
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
        public ICollection<ServiceTypeElement> ServiceTypes
        {
            get;
            set;
        }

        [XmlElement(ElementName = "CodePackage")]
        public ICollection<CodePackageElement> CodePackages
        {
            get;
            set;
        }

        [XmlElement(ElementName = "ConfigPackage")]
        public ICollection<ConfigurationPackageElement> ConfigPackages
        {
            get;
            set;
        }

        [XmlElement(ElementName = "DataPackage")]
        public ICollection<DataPackageElement> DataPackages
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
            this.ConfigPackages = new PackageElementCollection<ConfigurationPackageElement>(this);
            this.DataPackages = new PackageElementCollection<DataPackageElement>(this);
        }
    }
}