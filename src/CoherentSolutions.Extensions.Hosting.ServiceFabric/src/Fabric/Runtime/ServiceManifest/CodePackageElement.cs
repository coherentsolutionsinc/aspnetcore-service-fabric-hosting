using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest
{
    public class CodePackageElement : PackageElement
    {
        [XmlElement(ElementName = "EnvironmentVariables")]
        public EnvironmentVariableElement[] EnvironmentVariables
        {
            get; 
            set;
        }
    }
}