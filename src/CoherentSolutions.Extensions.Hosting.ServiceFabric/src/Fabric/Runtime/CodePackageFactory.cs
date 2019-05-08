using System;
using System.Fabric;
using System.Fabric.Description;
using System.IO;
using System.Reflection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class CodePackageFactory : IPackageFactory<CodePackageElement, CodePackage>
    {
        private static readonly Lazy<ConstructorInfo> ctor;
        private static readonly Lazy<PropertyInfo> path;
        private static readonly Lazy<PropertyInfo> descr;

        private static readonly Lazy<ConstructorInfo> descrCtor;
        private static readonly Lazy<PropertyInfo> descrPath;
        private static readonly Lazy<PropertyInfo> descrName;
        private static readonly Lazy<PropertyInfo> descrVersion;
        private static readonly Lazy<PropertyInfo> descrServiceManifestName;
        private static readonly Lazy<PropertyInfo> descrServiceManifestVersion;

        private readonly ServiceManifestElement manifest;
        private readonly string packageRootPath;

        static CodePackageFactory()
        {
            ctor = new Lazy<ConstructorInfo>(() => typeof(CodePackage).GetNonPublicConstructor(), true);
            path = new Lazy<PropertyInfo>(() => typeof(CodePackage).GetProperty("Path"), true);
            descr = new Lazy<PropertyInfo>(() => typeof(CodePackage).GetProperty("Description"), true);

            descrCtor = new Lazy<ConstructorInfo>(() => typeof(CodePackageDescription).GetNonPublicConstructor(), true);
            descrPath = new Lazy<PropertyInfo>(() => typeof(CodePackageDescription).GetProperty("Path"), true);
            descrName = new Lazy<PropertyInfo>(() => typeof(CodePackageDescription).GetProperty("Name"), true);
            descrVersion = new Lazy<PropertyInfo>(() => typeof(CodePackageDescription).GetProperty("Version"), true);
            descrServiceManifestName = new Lazy<PropertyInfo>(() => typeof(CodePackageDescription).GetProperty("ServiceManifestName"), true);
            descrServiceManifestVersion = new Lazy<PropertyInfo>(() => typeof(CodePackageDescription).GetProperty("ServiceManifestVersion"), true);
        }

        public CodePackageFactory(
            ServiceManifestElement manifest,
            string packageRootPath)
        {
            if (string.IsNullOrWhiteSpace(packageRootPath))
            {
                throw new ArgumentException(nameof(packageRootPath));
            }

            this.manifest = manifest;
            this.packageRootPath = packageRootPath;
        }

        public CodePackage Create(CodePackageElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (descrCtor.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackageDescription), ".ctor()");
            }
            if (descrPath.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackageDescription), "Path");
            }
            if (descrName.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackageDescription),"Name");
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

            if (ctor.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackage), ".ctor()");
            }
            if (path.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackage), nameof(CodePackage.Path));
            }

            var packagePath = Path.Combine(this.packageRootPath, element.Name);

            var description = (CodePackageDescription)descrCtor.Value.Invoke(null);
            descrPath.Value.SetValue(description, packagePath);
            descrName.Value.SetValue(description, element.Name);
            descrVersion.Value.SetValue(description, element.Version);
            descrServiceManifestName.Value.SetValue(description, manifest.Name);
            descrServiceManifestVersion.Value.SetValue(description, manifest.Version);

            var package = (CodePackage)ctor.Value.Invoke(null);
            descr.Value.SetValue(package, description);
            path.Value.SetValue(package, packagePath);

            return package;
        }
    }
}
