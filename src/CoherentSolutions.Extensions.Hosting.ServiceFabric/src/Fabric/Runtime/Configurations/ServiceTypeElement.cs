using System.Xml.Serialization;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public abstract class ServiceTypeElement
    {
        [XmlAttribute(AttributeName = "ServiceTypeName")]
        public string ServiceTypeName
        {
            get;
            set;
        }

        public abstract ServiceTypeElementKind Kind
        {
            get;
        }
    }
}