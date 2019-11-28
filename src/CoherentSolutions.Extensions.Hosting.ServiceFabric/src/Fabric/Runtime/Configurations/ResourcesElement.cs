using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class ResourcesElement
    {
        [XmlArray(ElementName = "Endpoints")]
        [XmlArrayItem(ElementName = "Endpoint")]
        public List<EndpointElement> Endpoints
        {
            get;
            set;
        }
    }
}