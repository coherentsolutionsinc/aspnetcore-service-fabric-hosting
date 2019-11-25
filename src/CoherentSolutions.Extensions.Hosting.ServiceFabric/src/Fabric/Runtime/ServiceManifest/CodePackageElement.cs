using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest
{
    public class CodePackageElement : PackageElement
    {
        [XmlArray(ElementName = "EnvironmentVariables")]
        [XmlArrayItem(ElementName = "EnvironmentVariable")]
        public ICollection<EnvironmentVariableElement> EnvironmentVariables
        {
            get; 
            set;
        }
    }
}