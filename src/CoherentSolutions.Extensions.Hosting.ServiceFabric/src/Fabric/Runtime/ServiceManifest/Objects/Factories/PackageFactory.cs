using System;
using System.Fabric.Description;
using System.IO;
using System.Reflection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public abstract class PackageFactory<TElement, TPackage> : IPackageFactory<TElement, TPackage>
    {
        private static readonly Lazy<PropertyInfo> descrPath;
        private static readonly Lazy<PropertyInfo> descrName;
        private static readonly Lazy<PropertyInfo> descrVersion;
        private static readonly Lazy<PropertyInfo> descrServiceManifestName;
        private static readonly Lazy<PropertyInfo> descrServiceManifestVersion;

        private readonly ServiceManifest manifest;

        static PackageFactory()
        {
            descrPath = new Lazy<PropertyInfo>(() => typeof(PackageDescription).GetProperty("Path"), true);
            descrName = new Lazy<PropertyInfo>(() => typeof(PackageDescription).GetProperty("Name"), true);
            descrVersion = new Lazy<PropertyInfo>(() => typeof(PackageDescription).GetProperty("Version"), true);
            descrServiceManifestName = new Lazy<PropertyInfo>(() => typeof(PackageDescription).GetProperty("ServiceManifestName"), true);
            descrServiceManifestVersion = new Lazy<PropertyInfo>(() => typeof(PackageDescription).GetProperty("ServiceManifestVersion"), true);
        }

        protected PackageFactory(
            ServiceManifest manifest)
        {
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        protected void InitializePackageDescription(
            PackageElement element,
            PackageDescription description)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (description is null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (descrPath.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackageDescription), "Path");
            }
            if (descrName.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackageDescription), "Name");
            }
            if (descrVersion.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackageDescription), "Version");
            }
            if (descrServiceManifestName.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackageDescription), "ServiceManifestName");
            }
            if (descrServiceManifestVersion.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackageDescription), "ServiceManifestVersion");
            }

            var path = Path.Combine(this.manifest.PackageRoot, element.Name);

            descrPath.Value.SetValue(description, path);
            descrName.Value.SetValue(description, element.Name);
            descrVersion.Value.SetValue(description, element.Version);
            descrServiceManifestName.Value.SetValue(description, this.manifest.Name);
            descrServiceManifestVersion.Value.SetValue(description, this.manifest.Version);
        }

        public abstract TPackage Create(TElement element);
    }
}
