using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public abstract class PackageFactory<TPackageElement, TPackage, TPackageDescription>
        : IPackageFactory<TPackageElement, TPackage>
        where TPackageElement : PackageElement
        where TPackage : class
        where TPackageDescription : PackageDescription
    {
        private static readonly Lazy<ConstructorInfo> packageCtor;

        private static readonly Lazy<ConstructorInfo> packageDescrCtor;

        static PackageFactory()
        {
            packageCtor = typeof(TPackage).QueryConstructor(@public: false);
            packageDescrCtor = typeof(TPackageDescription).QueryConstructor(@public: false);
        }

        public TPackage Create(
            TPackageElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var packageDescriptionAccessor = new PackageDescriptionAccessor<TPackageDescription>(
                (TPackageDescription)packageDescrCtor.Value.Invoke(null))
            {
                Path = System.IO.Path.Combine(element.Manifest.PackageRoot, element.Name),
                Name = element.Name,
                Version = element.Version,
                ServiceManifestName = element.Manifest.Name,
                ServiceManifestVersion = element.Manifest.Version
            };

            this.InitializePackageDescription(packageDescriptionAccessor.Instance, element);

            var packageAccess = new PackageAccessor<TPackage, TPackageDescription>(
                (TPackage)packageCtor.Value.Invoke(null))
            {
                Path = packageDescriptionAccessor.Path,
                Description = packageDescriptionAccessor.Instance
            };

            this.InitializePackage(packageAccess.Instance, element);

            return packageAccess.Instance;
        }

        protected virtual void InitializePackageDescription(
            TPackageDescription packageDescription,
            TPackageElement element)
        {
        }

        protected virtual void InitializePackage(
            TPackage package,
            TPackageElement element)
        {
        }
    }
}