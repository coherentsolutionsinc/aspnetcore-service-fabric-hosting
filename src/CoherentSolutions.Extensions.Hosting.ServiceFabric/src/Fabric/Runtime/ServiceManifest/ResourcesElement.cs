using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest
{
    public class ResourcesElement
    {
        [XmlArray(ElementName = "Endpoints")]
        [XmlArrayItem(ElementName = "Endpoint")]
        public EndpointElement[] Endpoints
        {
            get; set;
        }
    }
}
