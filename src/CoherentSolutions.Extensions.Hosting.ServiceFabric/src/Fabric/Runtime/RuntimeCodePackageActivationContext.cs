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
    public class RuntimeCodePackageActivationContext : ICodePackageActivationContext
    {
        private const string WORK_DIRECTORY = "work";
        private const string LOG_DIRECTORY = "log";
        private const string TEMP_DIRECTORY = "temp";
        
        private readonly CodePackage codePackage;
        private readonly ServiceManifest.Objects.ServiceManifest serviceManifest;

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

        public event EventHandler<PackageAddedEventArgs<CodePackage>> CodePackageAddedEvent;
        public event EventHandler<PackageRemovedEventArgs<CodePackage>> CodePackageRemovedEvent;
        public event EventHandler<PackageModifiedEventArgs<CodePackage>> CodePackageModifiedEvent;
        public event EventHandler<PackageAddedEventArgs<ConfigurationPackage>> ConfigurationPackageAddedEvent;
        public event EventHandler<PackageRemovedEventArgs<ConfigurationPackage>> ConfigurationPackageRemovedEvent;
        public event EventHandler<PackageModifiedEventArgs<ConfigurationPackage>> ConfigurationPackageModifiedEvent;
        public event EventHandler<PackageAddedEventArgs<DataPackage>> DataPackageAddedEvent;
        public event EventHandler<PackageRemovedEventArgs<DataPackage>> DataPackageRemovedEvent;
        public event EventHandler<PackageModifiedEventArgs<DataPackage>> DataPackageModifiedEvent;

        public RuntimeCodePackageActivationContext(
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

            var codePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (codePath is null)
            {
                throw new InvalidOperationException(
                    "Cannot located PackageRoot and ServiceManifest.xml " +
                    "because executing assembly isn't located in any of CodePackages");
            }

            var packageRoot = Path.GetDirectoryName(codePath) ?? Path.GetPathRoot(codePath);

            this.serviceManifest = new ServiceManifest.Objects.ServiceManifest(
                Path.Combine(
                    packageRoot, 
                    "ServiceManifest.xml"));
            
            this.codePackage = this.serviceManifest.CodePackages[Path.GetFileName(codePath)];
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ApplicationPrincipalsDescription GetApplicationPrincipals()
        {
            return this.serviceManifest.ApplicationPrincipalsDescription;
        }

        public IList<string> GetCodePackageNames()
        {
            return this.serviceManifest.CodePackages.Select(package => package.Description.Name).ToArray();
        }

        public CodePackage GetCodePackageObject(string packageName)
        {
            return this.serviceManifest.CodePackages[packageName];
        }

        public IList<string> GetConfigurationPackageNames()
        {
            return this.serviceManifest.ConfigPackages.Select(package => package.Description.Name).ToArray();
        }

        public ConfigurationPackage GetConfigurationPackageObject(string packageName)
        {
            return this.serviceManifest.ConfigPackages[packageName];
        }

        public IList<string> GetDataPackageNames()
        {
            return this.serviceManifest.DataPackages.Select(package => package.Description.Name).ToArray();
        }

        public DataPackage GetDataPackageObject(string packageName)
        {
            return this.serviceManifest.DataPackages[packageName];
        }

        public EndpointResourceDescription GetEndpoint(string endpointName)
        {
            return this.serviceManifest.EndpointResources[endpointName];
        }

        public string GetServiceManifestName()
        {
            return this.serviceManifest.Name;
        }

        public string GetServiceManifestVersion()
        {
            return this.serviceManifest.Version;
        }

        public KeyedCollection<string, ServiceTypeDescription> GetServiceTypes()
        {
            return this.serviceManifest.ServiceTypes;
        }

        public KeyedCollection<string, ServiceGroupTypeDescription> GetServiceGroupTypes()
        {
            return null;
        }

        public KeyedCollection<string, EndpointResourceDescription> GetEndpoints()
        {
            return this.serviceManifest.EndpointResources;
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
