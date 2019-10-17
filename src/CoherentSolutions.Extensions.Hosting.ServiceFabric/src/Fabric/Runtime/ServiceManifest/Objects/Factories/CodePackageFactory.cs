using System;
using System.Fabric;
using System.Fabric.Description;
using System.Reflection;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Common.Extensions;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories
{
    public class CodePackageFactory : PackageFactory<CodePackageElement, CodePackage>
    {
        private static readonly Lazy<ConstructorInfo> ctor;

        private static readonly Lazy<PropertyInfo> path;

        private static readonly Lazy<PropertyInfo> descr;

        private static readonly Lazy<ConstructorInfo> descrCtor;

        static CodePackageFactory()
        {
            ctor = typeof(CodePackage).Query().Constructor().NonPublic().Instance().GetLazy();
            path = typeof(CodePackage).Query().Property(nameof(CodePackage.Path)).Public().Instance().GetLazy();
            descr = typeof(CodePackage).Query().Property(nameof(CodePackage.Description)).Public().Instance().GetLazy();

            descrCtor = typeof(CodePackageDescription).Query().Constructor().NonPublic().Instance().GetLazy();
        }

        public CodePackageFactory(
            ServiceManifest manifest)
            : base(manifest)
        {
        }

        public override CodePackage Create(
            CodePackageElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (ctor.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackage), ".ctor()");
            }

            if (path.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackage), "Path");
            }

            if (descrCtor.Value is null)
            {
                throw new MissingMemberException(nameof(CodePackageDescription), ".ctor()");
            }

            var description = (CodePackageDescription)descrCtor.Value.Invoke(null);
            this.InitializePackageDescription(element, description);

            var package = (CodePackage)ctor.Value.Invoke(null);
            descr.Value.SetValue(package, description);
            path.Value.SetValue(package, description.Path);

            return package;
        }
    }
}