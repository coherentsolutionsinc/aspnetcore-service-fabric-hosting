using System;
using System.Fabric;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class DataPackageFactory : PackageFactory<DataPackageElement, DataPackage, DataPackageDescription>
    {
        protected override PackageAccessor<DataPackage, DataPackageDescription> CreatePackage(
            DataPackage package)
        {
            return new DataPackageAccessor(package);
        }

        protected override PackageDescriptionAccessor<DataPackageDescription> CreatePackageDescription(
            DataPackageDescription packageDescription)
        {
            return new DataPackageDescriptionAccessor(packageDescription);
        }
    }
}