using System;
using System.Fabric;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class DataPackageFactory : PackageFactory<DataPackageElement, DataPackage>
    {
        private static readonly Lazy<ConstructorInfo> ctor;

        private static readonly Lazy<PropertyInfo> path;

        private static readonly Lazy<PropertyInfo> descr;

        private static readonly Lazy<ConstructorInfo> descrCtor;

        static DataPackageFactory()
        {
            ctor = typeof(DataPackage).Query().Constructor().NonPublic().Instance().GetLazy();


            // typeof(DataPackage).Query().Public().Instance().Constructor().ExecuteLazy();


            path = typeof(DataPackage).Query().Property(nameof(DataPackage.Path)).Public().Instance().GetLazy();
            descr = typeof(DataPackage).Query().Property(nameof(DataPackage.Description)).Public().Instance().GetLazy();

            descrCtor = typeof(DataPackage).Query().Constructor().NonPublic().Instance().GetLazy();
        }

        public DataPackageFactory(
            ServiceManifest manifest)
            : base(manifest)
        {
        }

        public override DataPackage Create(
            DataPackageElement element)
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
                throw new MissingMemberException(nameof(DataPackage), "Path");
            }

            if (descrCtor.Value is null)
            {
                throw new MissingMemberException(nameof(DataPackageDescription), ".ctor()");
            }

            var description = (DataPackageDescription)descrCtor.Value.Invoke(null);
            this.InitializePackageDescription(element, description);

            var package = (DataPackage)ctor.Value.Invoke(null);
            descr.Value.SetValue(package, description);
            path.Value.SetValue(package, description.Path);

            return package;
        }
    }
}