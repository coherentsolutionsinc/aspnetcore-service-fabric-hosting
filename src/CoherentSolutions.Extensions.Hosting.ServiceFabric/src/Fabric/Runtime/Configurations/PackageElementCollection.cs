using System.Collections;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations
{
    public class PackageElementCollection<T> : ICollection<T>
        where T : PackageElement
    {
        private readonly List<T> collection;

        private readonly ServiceManifestElement manifest;

        public int Count => this.collection.Count;

        public bool IsReadOnly => ((ICollection<T>)this.collection).IsReadOnly;

        public PackageElementCollection(
            ServiceManifestElement parent)
        {
            this.collection = new List<T>();
            this.manifest = parent ?? throw new System.ArgumentNullException(nameof(parent));
        }

        public void Add(
            T item)
        {
            this.collection.Add(item);

            item.Manifest = this.manifest;
        }

        public void Clear()
        {
            foreach (var item in this.collection)
            {
                item.Manifest = null;
            }

            this.collection.Clear();
        }

        public bool Contains(
            T item)
        {
            return this.collection.Contains(item);
        }

        public void CopyTo(
            T[] array,
            int arrayIndex)
        {
            this.collection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((ICollection<T>)this.collection).GetEnumerator();
        }

        public bool Remove(
            T item)
        {
            if (this.collection.Remove(item))
            {
                item.Manifest = null;
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<T>)this.collection).GetEnumerator();
        }
    }
}