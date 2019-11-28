using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.Configurations;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ActivationContexts
{
    public class CodePackageActivationContextReader : ICodePackageActivationContextReader
    {
        private const string CODE_PACKAGE_NAME = "Code";

        private const string CODE_PACKAGE_VERSION = "1.0.0";

        public ICodePackageActivationContext Read(
            ServiceManifestElement manifest)
        {
            if (manifest is null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }

            var activeCodePackage = new CodePackageFactory()
               .Create(
                    new CodePackageElement
                    {
                        Manifest = manifest,
                        Name = CODE_PACKAGE_NAME,
                        Version = CODE_PACKAGE_VERSION
                    });

            return new CodePackageActivationContext(
                manifest.Name,
                manifest.Version,
                activeCodePackage,
                new ApplicationPrincipalsDescription(),
                CreatePackagesFrom(manifest.ConfigurationPackages, new ConfigurationPackageFactory()),
                CreatePackagesFrom(manifest.DataPackages, new DataPackageFactory()),
                CreateServiceTypesDescriptionsFrom(manifest.ServiceTypes),
                CreateServiceEndpointsFrom(manifest.Resources.Endpoints));
        }

        private static IEnumerable<TPackage> CreatePackagesFrom<TElement, TPackage>(
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

        private static IEnumerable<ServiceTypeDescription> CreateServiceTypesDescriptionsFrom(
            IEnumerable<ServiceTypeElement> elements)
        {
            if (elements is null)
            {
                yield break;
            }

            foreach (var element in elements)
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

        private static IEnumerable<EndpointResourceDescription> CreateServiceEndpointsFrom(
            IEnumerable<EndpointElement> elements)
        {
            if (elements is null)
            {
                yield break;
            }

            foreach (var element in elements)
            {
                var description = new EndpointResourceDescriptionAccessor(new EndpointResourceDescription())
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

                if (int.TryParse(element.Port, out var port))
                {
                    description.Port = port;
                }

                yield return description.Instance;
            }
        }
    }
}