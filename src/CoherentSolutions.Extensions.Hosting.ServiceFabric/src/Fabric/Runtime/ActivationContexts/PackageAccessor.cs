using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class PackageAccessor<TPackage, TPackageDescription>
        where TPackage : class
        where TPackageDescription : PackageDescription
    {
        private static readonly Lazy<PropertyInfo> path;

        private static readonly Lazy<PropertyInfo> description;

        public TPackage Instance { get; }

        public string Path
        {
            get => (string) path.Value.GetValue(this.Instance);
            set => path.Value.SetValue(this.Instance, value);
        }

        public TPackageDescription Description
        {
            get => (TPackageDescription) description.Value.GetValue(this.Instance);
            set => description.Value.SetValue(this.Instance, value);
        }

        static PackageAccessor()
        {
            path = typeof(TPackage).QueryProperty("Path");
            description = typeof(TPackage).QueryProperty("Description");
        }

        public PackageAccessor(
            TPackage instance)
        {
            this.Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}