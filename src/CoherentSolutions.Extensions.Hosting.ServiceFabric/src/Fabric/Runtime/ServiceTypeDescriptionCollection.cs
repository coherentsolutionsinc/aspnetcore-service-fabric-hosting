using System.Collections.ObjectModel;
using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class ServiceTypeDescriptionCollection : KeyedCollection<string, ServiceTypeDescription>
    {
        protected override string GetKeyForItem(ServiceTypeDescription item)
        {
            return item.ServiceTypeName;
        }
    }
}
