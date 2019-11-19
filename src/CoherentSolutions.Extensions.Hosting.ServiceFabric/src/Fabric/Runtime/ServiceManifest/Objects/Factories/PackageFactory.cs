using System;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
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
            packageCtor = typeof(TPackage).Query().Constructor().NonPublic().Instance().GetLazy();
            packageDescrCtor = typeof(TPackageDescription).Query().Constructor().NonPublic().Instance().GetLazy();
        }

        protected abstract PackageAccessor<TPackage, TPackageDescription> CreatePackage(
            TPackage package);

        protected abstract PackageDescriptionAccessor<TPackageDescription> CreatePackageDescription(
            TPackageDescription packageDescription);

        public TPackage Create(
            TPackageElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var packageDescription = this.CreatePackageDescription(
                (TPackageDescription)packageDescrCtor.Value.Invoke(null));

            packageDescription.Name = element.Name;
            packageDescription.Version = element.Version;
            packageDescription.ServiceManifestName = element.Manifest.Name;
            packageDescription.ServiceManifestVersion = element.Manifest.Version;

            var package = this.CreatePackage(
                (TPackage)packageCtor.Value.Invoke(null));
            
            package.Description = packageDescription.Instance;

            return package.Instance;
        }
    }
}