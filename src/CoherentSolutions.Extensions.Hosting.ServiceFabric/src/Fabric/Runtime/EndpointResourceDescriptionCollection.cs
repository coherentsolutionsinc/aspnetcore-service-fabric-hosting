using System.Collections.ObjectModel;
using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class EndpointResourceDescriptionCollection : KeyedCollection<string, EndpointResourceDescription>
    {
        protected override string GetKeyForItem(EndpointResourceDescription item)
        {
            return item.Name;
        }
    }
}
