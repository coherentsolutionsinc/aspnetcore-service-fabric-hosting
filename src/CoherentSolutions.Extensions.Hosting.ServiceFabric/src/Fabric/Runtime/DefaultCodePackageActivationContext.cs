using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Fabric.Health;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime.ServiceManifest;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class DefaultCodePackageActivationContext : ICodePackageActivationContext
    {
        private static readonly XmlSerializer serializer;

        private readonly string serviceManifestName;
        private readonly string serviceManifestVersion;

        private readonly ServiceTypeDescriptionCollection serviceTypeDescriptions;
        private readonly EndpointResourceDescriptionCollection endpointDescriptions;
        private readonly ApplicationPrincipalsDescription applicationPrincipalsDescription;

        private readonly CodePackage codePackage;

        private readonly string[] codePackageNames;
        private readonly Dictionary<string, CodePackage> codePackages;

        private readonly string[] configPackageNames;
        private readonly Dictionary<string, ConfigurationPackage> configPackages;

        private readonly string[] dataPackageNames;
        private readonly Dictionary<string, DataPackage> dataPackages;

        public string ApplicationName
        {
            get;
        }

        public string ApplicationTypeName
        {
            get;
        }

        public string ContextId
        {
            get;
        }

        public string WorkDirectory
        {
            get;
        }

        public string LogDirectory
        {
            get;
        }

        public string TempDirectory
        {
            get;
        }


        public string CodePackageName
        {
            get
            {
                return this.codePackage.Description.Name;
            }
        }

        public string CodePackageVersion
        {
            get
            {
                return this.codePackage.Description.Version;
            }
        }

        public event EventHandler<PackageAddedEventArgs<CodePackage>> CodePackageAddedEvent;
        public event EventHandler<PackageRemovedEventArgs<CodePackage>> CodePackageRemovedEvent;
        public event EventHandler<PackageModifiedEventArgs<CodePackage>> CodePackageModifiedEvent;
        public event EventHandler<PackageAddedEventArgs<ConfigurationPackage>> ConfigurationPackageAddedEvent;
        public event EventHandler<PackageRemovedEventArgs<ConfigurationPackage>> ConfigurationPackageRemovedEvent;
        public event EventHandler<PackageModifiedEventArgs<ConfigurationPackage>> ConfigurationPackageModifiedEvent;
        public event EventHandler<PackageAddedEventArgs<DataPackage>> DataPackageAddedEvent;
        public event EventHandler<PackageRemovedEventArgs<DataPackage>> DataPackageRemovedEvent;
        public event EventHandler<PackageModifiedEventArgs<DataPackage>> DataPackageModifiedEvent;

        static DefaultCodePackageActivationContext()
        {
            serializer = new XmlSerializer(typeof(ServiceManifestElement), "http://schemas.microsoft.com/2011/01/fabric");
        }

        public DefaultCodePackageActivationContext(
            string applicationName,
            string applicationTypename,
            string contextId)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
            {
                throw new ArgumentException(nameof(applicationName));
            }

            if (string.IsNullOrWhiteSpace(applicationTypename))
            {
                throw new ArgumentException(nameof(applicationTypename));
            }

            if (string.IsNullOrWhiteSpace(contextId))
            {
                throw new ArgumentException(nameof(contextId));
            }

            this.ApplicationName = applicationName;
            this.ApplicationTypeName = applicationTypename;
            this.ContextId = contextId;

            var basePath = FindBasePath();

            this.WorkDirectory = Path.Combine(basePath, "work");
            this.LogDirectory = Path.Combine(basePath, "log");
            this.TempDirectory = Path.Combine(basePath, "temp");

            FindPackageRootAndServiceManifestPaths(out var serviceManifestPath, out var packageRootPath);

            var manifest = ReadServiceManifest(serviceManifestPath);

            this.serviceManifestName = manifest.Name;
            this.serviceManifestVersion = manifest.Version;

            if (manifest.ServiceTypes?.Length > 0)
            {
                ReadServiceTypesDescriptions(manifest.ServiceTypes, out this.serviceTypeDescriptions);
            }

            this.applicationPrincipalsDescription = new ApplicationPrincipalsDescription();

            if (manifest.CodePackages?.Length > 0)
            {
                ReadServicePackages(
                    manifest.CodePackages,
                    new CodePackageFactory(manifest, packageRootPath),
                    out this.codePackageNames,
                    out this.codePackages);

                FindCodePackage(this.codePackages, out this.codePackage);
            }

            if (manifest.ConfigPackages?.Length > 0)
            {
                ReadServicePackages(
                    manifest.ConfigPackages,
                    new ConfigurationPackageFactory(packageRootPath),
                    out this.configPackageNames,
                    out this.configPackages);
            }

            if (manifest.DataPackages?.Length > 0)
            {
                ReadServicePackages(
                    manifest.DataPackages,
                    new DataPackageFactory(packageRootPath),
                    out this.dataPackageNames,
                    out this.dataPackages);
            }

            if (manifest.Resources?.Endpoints?.Length > 0)
            {
                ReadServiceEndpoints(
                    manifest.Resources.Endpoints,
                    out this.endpointDescriptions);
            }
        }

        private static string FindBasePath()
        {
            return Environment.CurrentDirectory;
        }

        /// <summary>
        /// This method tries to find nearest ServiceManifest.xml file by probing for existance:
        /// - <pwd>\ServiceManifest.xml / if found: set path
        /// - <pwd>\PackageRoot / if found try to search for ServiceManifest.xml, otherwise return false.
        ///
        /// When both of these conditions are false move upward and repeat.
        /// </summary>
        private static void FindPackageRootAndServiceManifestPaths(
            out string serviceManifestPath,
            out string packageRootPath)
        {
            var brk = false;
            var currentDir = Environment.CurrentDirectory;
            for (; ; )
            {
                var file = Path.Combine(currentDir, "ServiceManifest.xml");
                if (File.Exists(file))
                {
                    serviceManifestPath = file;
                    packageRootPath = currentDir;
                    return;
                }

                if (brk)
                {
                    break;
                }

                var directory = Path.Combine(currentDir, "PackageRoot");
                if (Directory.Exists(directory))
                {
                    currentDir = directory;
                    brk = true;
                }
                else
                {
                    currentDir = Path.GetDirectoryName(currentDir);
                    if (currentDir is null)
                    {
                        break;
                    }
                }
            }
            throw new InvalidOperationException("Unable to find PackageRoot and ServiceManifest.xml");
        }

        private static ServiceManifestElement ReadServiceManifest(
            string path)
        {
            using (var fs = File.OpenRead(path))
            {
                return (ServiceManifestElement)serializer.Deserialize(fs);
            }
        }

        private static void ReadServiceTypesDescriptions(
            IReadOnlyList<ServiceTypeElement> elements,
            out ServiceTypeDescriptionCollection descriptions)
        {
            descriptions = new ServiceTypeDescriptionCollection();
            foreach (var element in elements)
            {
                switch (element.Kind)
                {
                    case ServiceTypeElementKind.Stateless:
                        descriptions.Add(
                            new StatelessServiceTypeDescription()
                            {
                                ServiceTypeName = element.ServiceTypeName
                            });
                        break;
                }
            }
        }

        private static void ReadServicePackages<TElement, TPackage>(
            IReadOnlyList<TElement> elements,
            IPackageFactory<TElement, TPackage> factory,
            out string[] names,
            out Dictionary<string, TPackage> packages)
                where TElement : PackageElement
        {
            names = new string[elements.Count];
            packages = new Dictionary<string, TPackage>(elements.Count, StringComparer.Ordinal);

            for (var i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                var name = element.Name;

                names[i] = element.Name;
                packages.Add(name, factory.Create(element));
            }
        }

        private static void ReadServiceEndpoints(
            IReadOnlyList<EndpointElement> elements,
            out EndpointResourceDescriptionCollection descriptions)
        {
            descriptions = new EndpointResourceDescriptionCollection();
            foreach (var element in elements)
            {
                var description = new EndpointResourceDescription()
                {
                    Name = element.Name ?? string.Empty,
                };

                if (Enum.TryParse<EndpointProtocol>(element.Protocol, out var protocol))
                {
                    description.Protocol = protocol;
                }
                if (Enum.TryParse<EndpointType>(element.Type, out var type))
                {
                    description.EndpointType = type;
                }

                descriptions.Add(description);
            }
        }

        /// <summary>
        ///     If there is only one code package defined in ServiceManifest.xml (which would be the 90% case) then
        ///     current method return this code package.
        ///     
        ///     Otherwise it tries to match assembly location with code package location and 
        ///     if it didn't work (and most of the time it wouldn't) then it takes a random packages.
        /// </summary>
        private static void FindCodePackage(
            IReadOnlyDictionary<string, CodePackage> packages,
            out CodePackage package)
        {
            if (packages.Count > 1)
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                foreach (var kv in packages)
                {
                    if (executingAssembly.Location.StartsWith(kv.Value.Path))
                    {
                        package = kv.Value;
                        return;
                    }
                }
            }

            package = packages.Values.First();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ApplicationPrincipalsDescription GetApplicationPrincipals()
        {
            return this.applicationPrincipalsDescription;
        }

        public IList<string> GetCodePackageNames()
        {
            return this.codePackageNames ?? Array.Empty<string>();
        }

        public CodePackage GetCodePackageObject(string packageName)
        {
            if (this.codePackages is null || !this.codePackages.TryGetValue(packageName, out var package))
            {
                throw new FabricElementNotFoundException(FabricErrorCode.CodePackageNotFound);
            }

            return package;
        }

        public IList<string> GetConfigurationPackageNames()
        {
            return this.configPackageNames ?? Array.Empty<string>();
        }

        public ConfigurationPackage GetConfigurationPackageObject(string packageName)
        {
            if (this.configPackages is null || !this.configPackages.TryGetValue(packageName, out var package))
            {
                throw new FabricElementNotFoundException(FabricErrorCode.ConfigurationPackageNotFound);
            }

            return package;
        }

        public IList<string> GetDataPackageNames()
        {
            return this.dataPackageNames ?? Array.Empty<string>();
        }

        public DataPackage GetDataPackageObject(string packageName)
        {
            if (this.dataPackages is null || !this.dataPackages.TryGetValue(packageName, out var package))
            {
                throw new FabricElementNotFoundException(FabricErrorCode.DataPackageNotFound);
            }

            return package;
        }

        public EndpointResourceDescription GetEndpoint(string endpointName)
        {
            if (string.IsNullOrEmpty(endpointName))
            {
                throw new ArgumentNullException(nameof(endpointName));
            }

            if (this.endpointDescriptions is null || !this.endpointDescriptions.Contains(endpointName))
            {
                throw new FabricElementNotFoundException(FabricErrorCode.EndpointResourceNotFound);
            }

            return this.endpointDescriptions[endpointName];
        }

        public KeyedCollection<string, EndpointResourceDescription> GetEndpoints()
        {
            return this.endpointDescriptions;
        }

        public KeyedCollection<string, ServiceGroupTypeDescription> GetServiceGroupTypes()
        {
            return null;
        }

        public string GetServiceManifestName()
        {
            return this.serviceManifestName;
        }

        public string GetServiceManifestVersion()
        {
            return this.serviceManifestVersion;
        }

        public KeyedCollection<string, ServiceTypeDescription> GetServiceTypes()
        {
            return this.serviceTypeDescriptions;
        }

        public void ReportApplicationHealth(HealthInformation healthInfo)
        {
        }

        public void ReportApplicationHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions)
        {
        }

        public void ReportDeployedApplicationHealth(HealthInformation healthInfo)
        {
        }

        public void ReportDeployedApplicationHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions)
        {
        }

        public void ReportDeployedServicePackageHealth(HealthInformation healthInfo)
        {
        }

        public void ReportDeployedServicePackageHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions)
        {
        }
    }
}
