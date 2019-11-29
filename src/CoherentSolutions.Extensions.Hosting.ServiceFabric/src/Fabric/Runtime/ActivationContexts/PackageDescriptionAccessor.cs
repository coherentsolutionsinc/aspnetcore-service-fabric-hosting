using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class PackageDescriptionAccessor<TPackageDescription>
        where TPackageDescription : PackageDescription
    {
        private static readonly Lazy<PropertyInfo> path;

        private static readonly Lazy<PropertyInfo> name;

        private static readonly Lazy<PropertyInfo> version;

        private static readonly Lazy<PropertyInfo> serviceManifestName;

        private static readonly Lazy<PropertyInfo> serviceManifestVersion;

        public TPackageDescription Instance
        {
            get;
        }

        public string Path
        {
            get => this.Instance.Path;
            set => path.Value.SetValue(this.Instance, value);
        }

        public string Name
        {
            get => this.Instance.Name;
            set => name.Value.SetValue(this.Instance, value);
        }

        public string Version
        {
            get => this.Instance.Version;
            set => version.Value.SetValue(this.Instance, value);
        }

        public string ServiceManifestName
        {
            get => this.Instance.ServiceManifestName;
            set => serviceManifestName.Value.SetValue(this.Instance, value);
        }

        public string ServiceManifestVersion
        {
            get => this.Instance.ServiceManifestVersion;
            set => serviceManifestVersion.Value.SetValue(this.Instance, value);
        }

        static PackageDescriptionAccessor()
        {
            path = typeof(PackageDescription).QueryProperty(nameof(PackageDescription.Path));
            name = typeof(PackageDescription).QueryProperty(nameof(PackageDescription.Name));
            version = typeof(PackageDescription).QueryProperty(nameof(PackageDescription.Version));
            serviceManifestName = typeof(PackageDescription).QueryProperty(nameof(PackageDescription.ServiceManifestName));
            serviceManifestVersion = typeof(PackageDescription).QueryProperty(nameof(PackageDescription.ServiceManifestVersion));
        }

        public PackageDescriptionAccessor(
            TPackageDescription instance)
        {
            this.Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}