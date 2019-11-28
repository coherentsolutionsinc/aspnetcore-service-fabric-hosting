using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric.Description;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class EndpointResourceDescriptionCollection : KeyedCollection<string, EndpointResourceDescription>
    {
        public EndpointResourceDescriptionCollection()
        {
        }

        public EndpointResourceDescriptionCollection(
            IEnumerable<EndpointResourceDescription> endpointResourceDescriptions)
        {
            if (endpointResourceDescriptions is null)
            {
                throw new ArgumentNullException(nameof(endpointResourceDescriptions));
            }

            foreach (var package in endpointResourceDescriptions)
            {
                this.Add(package);
            }
        }

        protected override string GetKeyForItem(
            EndpointResourceDescription item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return item.Name;
        }
    }
}