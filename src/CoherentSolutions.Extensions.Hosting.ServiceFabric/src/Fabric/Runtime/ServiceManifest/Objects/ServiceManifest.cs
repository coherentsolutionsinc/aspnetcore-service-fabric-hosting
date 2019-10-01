using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.IO;
using System.Xml.Serialization;

using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Collections;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects.Factories;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest.Objects
{
    public class ServiceManifest
    {
        private readonly ServiceTypeDescriptionCollection serviceTypeDescriptions;

        private readonly EndpointResourceDescriptionCollection endpointResourceDescriptions;

        private readonly CodePackageCollection codePackages;

        private readonly ConfigPackageCollection configPackages;

        private readonly DataPackageCollection dataPackages;

        public string PackageRoot
        {
            get;
        }

        public string Name
        {
            get;
        }

        public string Version
        {
            get;
        }

        public ApplicationPrincipalsDescription ApplicationPrincipalsDescription
        {
            get;
            private set;
        }

        public KeyedCollection<string, ServiceTypeDescription> ServiceTypes
        {
            get
            {
                return this.serviceTypeDescriptions;
            }
        }

        public KeyedCollection<string, EndpointResourceDescription> EndpointResources
        {
            get
            {
                return this.endpointResourceDescriptions;
            }
        }

        public KeyedCollection<string, CodePackage> CodePackages
        {
            get
            {
                return this.codePackages;
            }
        }

        public KeyedCollection<string, ConfigurationPackage> ConfigPackages
        {
            get
            {
                return this.configPackages;
            }
        }

        public KeyedCollection<string, DataPackage> DataPackages
        {
            get
            {
                return this.dataPackages;
            }
        }

        public ServiceManifest(
            string path)
            : this(path, ReadServiceManifest(path))
        {
        }

        public ServiceManifest(
            string path,
            ServiceManifestElement element)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            this.Name = element.Name;
            this.Version = element.Version;
            this.PackageRoot = Path.GetDirectoryName(path);
            this.ApplicationPrincipalsDescription = new ApplicationPrincipalsDescription();

            this.serviceTypeDescriptions = new ServiceTypeDescriptionCollection(
                ReadServiceTypesDescriptions(element.ServiceTypes));

            this.codePackages = new CodePackageCollection(
                ReadPackages(element.CodePackages, new CodePackageFactory(this)));

            this.configPackages = new ConfigPackageCollection(
                ReadPackages(element.ConfigPackages, new ConfigurationPackageFactory(this)));

            this.dataPackages = new DataPackageCollection(
                ReadPackages(element.DataPackages, new DataPackageFactory(this)));

            this.endpointResourceDescriptions = new EndpointResourceDescriptionCollection(
                ReadServiceEndpoints(element.Resources.Endpoints));
        }

        private static ServiceManifestElement ReadServiceManifest(
            string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            try
            {
                using (var stream = File.Open(path, FileMode.Open))
                {
                    var serializer = new XmlSerializer(
                        typeof(ServiceManifestElement),
                        "http://schemas.microsoft.com/2011/01/fabric");

                    return (ServiceManifestElement)serializer.Deserialize(stream);
                }
            }
            catch (FileNotFoundException exception)
            {
                throw new FileNotFoundException("Cannot read ServiceManifest.xml because it doesn't exist", path, exception);
            }
        }

        private static IEnumerable<StatelessServiceTypeDescription> ReadServiceTypesDescriptions(
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

        private static IEnumerable<EndpointResourceDescription> ReadServiceEndpoints(
            IEnumerable<EndpointElement> elements)
        {
            if (elements is null)
            {
                yield break;
            }

            foreach (var element in elements)
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