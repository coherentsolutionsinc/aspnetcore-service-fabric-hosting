using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Extensions
{
    public static class ServiceManifestElementExtensions
    {
        public static IEnumerable<ConfigurationPackage> ReadConfigurationPackages(
            this ServiceManifestElement @this)
        {
            return @this is null 
                ? Enumerable.Empty<ConfigurationPackage>()
                : ReadPackages(@this.ConfigPackages, new ConfigurationPackageFactory());
        }

        public static IEnumerable<DataPackage> ReadDataPackages(
            this ServiceManifestElement @this)
        {
            return @this is null 
                ? Enumerable.Empty<DataPackage>() 
                : ReadPackages(@this.DataPackages, new DataPackageFactory());
        }

        public static IEnumerable<ServiceTypeDescription> ReadServiceTypesDescriptions(
            this ServiceManifestElement @this)
        {
            if (@this is null)
            {
                yield break;
            }

            foreach (var element in @this.ServiceTypes)
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

        public static IEnumerable<EndpointResourceDescription> ReadServiceEndpoints(
            this ServiceManifestElement @this)
        {
            if (@this is null)
            {
                yield break;
            }

            foreach (var element in @this.Resources.Endpoints)
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
