using System;
using System.Fabric;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class ConfigurationPackageFactory : PackageFactory<ConfigPackageElement, ConfigurationPackage>
    {
        private static readonly Lazy<ConstructorInfo> ctor;

        private static readonly Lazy<PropertyInfo> path;

        private static readonly Lazy<PropertyInfo> descr;

        private static readonly Lazy<ConstructorInfo> descrCtor;

        static ConfigurationPackageFactory()
        {
            ctor = typeof(ConfigurationPackage).Query().Constructor().NonPublic().Instance().GetLazy();
            path = typeof(ConfigurationPackage).Query().Property(nameof(ConfigurationPackage.Path)).Public().Instance().GetLazy();
            descr = typeof(ConfigurationPackage).Query().Property(nameof(ConfigurationPackage.Description)).Public().Instance().GetLazy();

            descrCtor = typeof(ConfigurationPackageDescription).Query().Constructor().NonPublic().Instance().GetLazy();
        }

        public ConfigurationPackageFactory(
            ServiceManifest manifest)
            : base(manifest)
        {
        }

        public override ConfigurationPackage Create(
            ConfigPackageElement element)
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
                throw new MissingMemberException(nameof(ConfigurationPackage), "Path");
            }

            if (descrCtor.Value is null)
            {
                throw new MissingMemberException(nameof(ConfigurationPackageDescription), ".ctor()");
            }

            var description = (ConfigurationPackageDescription)descrCtor.Value.Invoke(null);
            this.InitializePackageDescription(element, description);

            var package = (ConfigurationPackage)ctor.Value.Invoke(null);
            descr.Value.SetValue(package, description);
            path.Value.SetValue(package, description.Path);

            return package;
        }
    }
}