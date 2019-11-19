using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Collections
{
    public class DataPackageCollection : KeyedCollection<string, DataPackage>
    {
        public DataPackageCollection(
            IEnumerable<DataPackage> packages)
        {
            if (packages is null)
            {
                throw new ArgumentNullException(nameof(packages));
            }

            foreach (var package in packages)
            {
                this.Add(package);
            }
        }

        protected override string GetKeyForItem(
            DataPackage item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return item.Description.Name;
        }
    }
}