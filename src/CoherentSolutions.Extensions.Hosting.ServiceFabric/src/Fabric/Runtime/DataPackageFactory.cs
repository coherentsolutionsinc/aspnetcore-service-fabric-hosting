using System;
using System.Fabric;
using System.IO;
using System.Reflection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class DataPackageFactory : IPackageFactory<DataPackageElement, DataPackage>
    {
        private readonly static Lazy<ConstructorInfo> ctor;
        private readonly static Lazy<PropertyInfo> path;

        private readonly string packageRootPath;

        static DataPackageFactory()
        {
            ctor = new Lazy<ConstructorInfo>(() => typeof(DataPackage).GetNonPublicConstructor(), true);
            path = new Lazy<PropertyInfo>(() => typeof(DataPackage).GetProperty(nameof(DataPackage.Path)), true);
        }

        public DataPackageFactory(string packageRootPath)
        {
            if (string.IsNullOrWhiteSpace(packageRootPath))
            {
                throw new ArgumentException(nameof(packageRootPath));
            }

            this.packageRootPath = packageRootPath;
        }

        public DataPackage Create(DataPackageElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (ctor.Value is null)
            {
                throw new MissingMemberException(nameof(DataPackage), ".ctor()");
            }
            if (path.Value is null)
            {
                throw new MissingMemberException(nameof(DataPackage), nameof(DataPackage.Path));
            }

            var instance = (DataPackage)ctor.Value.Invoke(null);
            path.Value.SetValue(instance, Path.Combine(this.packageRootPath, element.Name));

            return instance;
        }
    }
}
