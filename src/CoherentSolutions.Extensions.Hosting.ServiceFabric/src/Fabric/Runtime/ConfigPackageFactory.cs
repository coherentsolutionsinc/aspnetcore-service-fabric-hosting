using System;
using System.Fabric;
using System.IO;
using System.Reflection;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class ConfigurationPackageFactory : IPackageFactory<ConfigPackageElement, ConfigurationPackage>
    {
        private readonly static Lazy<ConstructorInfo> ctor;
        private readonly static Lazy<PropertyInfo> path;

        private readonly string packageRootPath;

        static ConfigurationPackageFactory()
        {
            ctor = new Lazy<ConstructorInfo>(() => typeof(ConfigurationPackage).GetNonPublicConstructor(), true);
            path = new Lazy<PropertyInfo>(() => typeof(ConfigurationPackage).GetProperty(nameof(ConfigurationPackage.Path)), true);
        }

        public ConfigurationPackageFactory(string packageRootPath)
        {
            if (string.IsNullOrWhiteSpace(packageRootPath))
            {
                throw new ArgumentException(nameof(packageRootPath));
            }

            this.packageRootPath = packageRootPath;
        }

        public ConfigurationPackage Create(ConfigPackageElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (ctor.Value is null)
            {
                throw new MissingMemberException(nameof(ConfigurationPackage), ".ctor()");
            }
            if (path.Value is null)
            {
                throw new MissingMemberException(nameof(ConfigurationPackage), nameof(ConfigurationPackage.Path));
            }

            var instance = (ConfigurationPackage)ctor.Value.Invoke(null);
            path.Value.SetValue(instance, Path.Combine(this.packageRootPath, element.Name));

            return instance;
        }
    }
}
