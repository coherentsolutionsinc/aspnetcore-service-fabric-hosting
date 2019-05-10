using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Collections
{
    public class ServiceTypeDescriptionCollection : KeyedCollection<string, ServiceTypeDescription>
    {
        public ServiceTypeDescriptionCollection(IEnumerable<ServiceTypeDescription> serviceTypeDescriptions)
        {
            if (serviceTypeDescriptions is null)
            {
                throw new ArgumentNullException(nameof(serviceTypeDescriptions));
            }

            foreach (var package in serviceTypeDescriptions)
            {
                this.Add(package);
            }
        }

        protected override string GetKeyForItem(ServiceTypeDescription item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return item.ServiceTypeName;
        }
    }
}
