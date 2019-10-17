using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Fabric.Health;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoherentSolutions.Extensions.Hosting.ServiceFabric.Fabric.Runtime
{
    public class GhostCodePackageActivationContext : ICodePackageActivationContext
    {
        private const string WORK_DIRECTORY = "work";

        private const string LOG_DIRECTORY = "log";

        private const string TEMP_DIRECTORY = "temp";

        private readonly CodePackage codePackage;

        private readonly ServiceManifest.Objects.ServiceManifest manifest;

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
            get
            {
                return Path.Combine(Environment.CurrentDirectory, WORK_DIRECTORY);
            }
        }

        public string LogDirectory
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, LOG_DIRECTORY);
            }
        }

        public string TempDirectory
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, TEMP_DIRECTORY);
            }
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

        public GhostCodePackageActivationContext(
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

            this.manifest = GetManifest();
            this.codePackage = null;
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ApplicationPrincipalsDescription GetApplicationPrincipals()
        {
            return this.manifest.ApplicationPrincipalsDescription;
        }

        public IList<string> GetCodePackageNames()
        {
            return this.manifest.CodePackages.Select(package => package.Description.Name).ToArray();
        }

        public CodePackage GetCodePackageObject(
            string packageName)
        {
            return this.manifest.CodePackages[packageName];
        }

        public IList<string> GetConfigurationPackageNames()
        {
            return this.manifest.ConfigPackages.Select(package => package.Description.Name).ToArray();
        }

        public ConfigurationPackage GetConfigurationPackageObject(
            string packageName)
        {
            return this.manifest.ConfigPackages[packageName];
        }

        public IList<string> GetDataPackageNames()
        {
            return this.manifest.DataPackages.Select(package => package.Description.Name).ToArray();
        }

        public DataPackage GetDataPackageObject(
            string packageName)
        {
            return this.manifest.DataPackages[packageName];
        }

        public EndpointResourceDescription GetEndpoint(
            string endpointName)
        {
            return this.manifest.EndpointResources[endpointName];
        }

        public string GetServiceManifestName()
        {
            return this.manifest.Name;
        }

        public string GetServiceManifestVersion()
        {
            return this.manifest.Version;
        }

        public KeyedCollection<string, ServiceTypeDescription> GetServiceTypes()
        {
            return this.manifest.ServiceTypes;
        }

        public KeyedCollection<string, ServiceGroupTypeDescription> GetServiceGroupTypes()
        {
            return null;
        }

        public KeyedCollection<string, EndpointResourceDescription> GetEndpoints()
        {
            return this.manifest.EndpointResources;
        }

        public void ReportApplicationHealth(
            HealthInformation healthInfo)
        {
        }

        public void ReportApplicationHealth(
            HealthInformation healthInfo,
            HealthReportSendOptions sendOptions)
        {
        }

        public void ReportDeployedApplicationHealth(
            HealthInformation healthInfo)
        {
        }

        public void ReportDeployedApplicationHealth(
            HealthInformation healthInfo,
            HealthReportSendOptions sendOptions)
        {
        }

        public void ReportDeployedServicePackageHealth(
            HealthInformation healthInfo)
        {
        }

        public void ReportDeployedServicePackageHealth(
            HealthInformation healthInfo,
            HealthReportSendOptions sendOptions)
        {
        }

        private static ServiceManifest.Objects.ServiceManifest GetManifest()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var current = location;

            var br = false;
            for (; !br;)
            {
                current = Path.GetDirectoryName(current);
                if (current is null)
                {
                    current = Path.GetPathRoot(location);
                    br = true;
                }

                var fi = new FileInfo(Path.Combine(current, "PackageRoot", "ServiceManifest.xml"));
                if (fi.Exists)
                {
                    return new ServiceManifest.Objects.ServiceManifest(fi.FullName);
                }
            }

            throw new InvalidOperationException(
                string.Format("Could not find 'ServiceManifest.xml' in {0} -> {0}{1}..{1}.", current, Path.DirectorySeparatorChar));
        }
    }
}