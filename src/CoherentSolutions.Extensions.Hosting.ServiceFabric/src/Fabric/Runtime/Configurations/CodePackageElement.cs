using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class CodePackageElement : PackageElement
    {
        [XmlArray(ElementName = "EnvironmentVariables")]
        [XmlArrayItem(ElementName = "EnvironmentVariable")]
        public List<EnvironmentVariableElement> EnvironmentVariables
        {
            get;
            set;
        }
    }
}