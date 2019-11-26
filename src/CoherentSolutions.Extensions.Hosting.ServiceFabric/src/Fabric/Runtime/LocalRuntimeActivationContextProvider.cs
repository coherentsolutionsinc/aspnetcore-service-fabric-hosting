using System;
using System.Fabric;
using System.Fabric.Description;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest;
using System.Linq;
using System.Collections.Generic;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public static class LocalRuntimeActivationContextProvider
    {
        private const string CODE_PACKAGE_NAME = "Code";

        private const string CODE_PACKAGE_VERSION = "1.0.0";

        public static ICodePackageActivationContext GetActivationContext()
        {
            var package = LocalRuntimeServicePackageProvider.GetPackage();
            var codePackage = new CodePackageFactory()
                .Create(new CodePackageElement
                {
                    Manifest = package.Manifest,
                    Name = CODE_PACKAGE_NAME,
                    Version = CODE_PACKAGE_VERSION
                });

            return new LocalRuntimeActivationContext(
                package.Manifest.Name,
                package.Manifest.Version,
                codePackage,
                new ApplicationPrincipalsDescription(),
                ReadConfigurationPackages(package.Manifest),
                ReadDataPackages(package.Manifest),
                ReadServiceTypesDescriptions(package.Manifest),
                ReadServiceEndpoints(package.Manifest));
        }

        private static IEnumerable<ConfigurationPackage> ReadConfigurationPackages(
            ServiceManifestElement manifest)
        {
            return manifest is null
                ? Enumerable.Empty<ConfigurationPackage>()
                : ReadPackages(manifest.ConfigurationPackages, new ConfigurationPackageFactory());
        }

        private static IEnumerable<DataPackage> ReadDataPackages(
            ServiceManifestElement manifest)
        {
            return manifest is null
                ? Enumerable.Empty<DataPackage>()
                : ReadPackages(manifest.DataPackages, new DataPackageFactory());
        }

        private static IEnumerable<ServiceTypeDescription> ReadServiceTypesDescriptions(
            ServiceManifestElement mnifest)
        {
            if (mnifest is null)
            {
                yield break;
            }

            foreach (var element in mnifest.ServiceTypes)
            {
                switch (element.Kind)
                {
                    case ServiceTypeElementKind.Stateless:
                        yield return new StatelessServiceTypeDescription()
                        {
                            ServiceTypeName = element.ServiceTypeName
                        };
                        break;
                }
            }
        }

        private static IEnumerable<EndpointResourceDescription> ReadServiceEndpoints(
            ServiceManifestElement manifest)
        {
            if (manifest is null)
            {
                yield break;
            }

            foreach (var element in manifest.Resources.Endpoints)
            {
                var description = new EndpointResourceDescription()
                {
                    Name = element.Name ?? string.Empty,
                    CodePackageName = element.CodePackageRef
                };

                if (Enum.TryParse<EndpointProtocol>(element.Protocol, out var protocol))
                {
                    description.Protocol = protocol;
                }

                if (Enum.TryParse<EndpointType>(element.Type, out var type))
                {
                    description.EndpointType = type;
                }

                yield return description;
            }
        }

        private static IEnumerable<TPackage> ReadPackages<TElement, TPackage>(
            IEnumerable<TElement> elements,
            IPackageFactory<TElement, TPackage> factory)
            where TElement : PackageElement
        {
            if (elements is null)
            {
                yield break;
            }

            foreach (var element in elements)
            {
                yield return factory.Create(element);
            }
        }
    }
}